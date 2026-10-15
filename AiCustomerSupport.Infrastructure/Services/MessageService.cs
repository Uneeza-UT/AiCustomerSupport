using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Application.DTOs.Notification;
using AiCustomerSupport.Application.Enums;
using AiCustomerSupport.Application.Exceptions;
using AiCustomerSupport.Application.Validations.Message;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Domain.Enums;
using AutoMapper;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAIService _aiService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IChatHubService _chatHubService;
        private readonly INotificationService _notificationService;

        public MessageService(IMapper mapper, 
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            ICurrentUserService currentUserService,
            IAIService aiService,
            IFileStorageService fileStorageService,
            IChatHubService chatHubService,
            INotificationService notificationService)
        {
            this._mapper = mapper;
            this._messageRepository = messageRepository;
            this._conversationRepository = conversationRepository;
            this._currentUserService = currentUserService;
            this._aiService = aiService;
            this._fileStorageService = fileStorageService;
            this._chatHubService = chatHubService;
            this._notificationService = notificationService;
        }


        public async Task<List<GetMessageDto>> GetAsync(int conversationId, PaginationDto dto)
        {
            var conversation = await GetConversationOrThrowAsync(conversationId);
            string userId = _currentUserService.UserId;

            string errorMessage = "You can only view messages of conversations you created or are assigned to.";

            EnsureConversationAccess(conversation, userId, errorMessage);

            var messages = await _messageRepository.GetPagedByConversationIdAndUserIdAsync(
                conversationId, userId, dto.PageNumber, dto.PageSize);

            return _mapper.Map<List<GetMessageDto>>(messages);
        }



        public async Task<GetMessageDto> SendAsync(int conversationId, CreateMessageDto dto)
        {
            string userId = _currentUserService.UserId;
            string role = _currentUserService.UserRole;

            // Validate the dto.
            var validator = new CreateMessageValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid message entity. ", validationResult);
            }


            var conversation = await GetConversationOrThrowAsync(conversationId);

            string errorMessage = "You can only send messages in conversations you created or are assigned to.";

            EnsureConversationAccess(conversation, userId, errorMessage);

            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
            bool isFirstMessage = messages.Count == 0;


            // Create a new message entity.
            var senderType = role switch
            {
                "Admin" => SenderType.Agent,

                "Agent" => SenderType.Agent,

                "Customer" => SenderType.Customer
            };

            var message = new Message
            {
                ConversationId = conversationId,
                SenderType = senderType,
                SenderId = userId,
                Content = dto.Content
            };


            // Add attachments to the message entity.
            if (dto.Attachments.Count != 0)
            {
                foreach (var file in dto.Attachments)
                {
                    var filePath = await _fileStorageService.SaveFileAsync(file);

                    var attachment = new Attachment
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        ContentType = file.ContentType
                    };

                    message.Attachments.Add(attachment);
                }
            }

            await _messageRepository.CreateAsync(message);

            Message? aiMessage = null;

            // Fetch and save AI response to Customer message.
            if (senderType == SenderType.Customer && conversation.Status != ConversationStatus.Escalated)
            {
                var aiResponse = await GetAIResponseAsync(conversationId, userId, message, isFirstMessage);

                if (isFirstMessage && !string.IsNullOrWhiteSpace(aiResponse.ConversationTitle))
                {
                    conversation.Title = aiResponse.ConversationTitle;
                    await _conversationRepository.SaveChangesAsync();
                }


                // Change conversation status to Escalated based on the AI Response.
                if (aiResponse.RequiresHumanAgent)
                {
                    conversation.Status = ConversationStatus.Escalated;
                    await _conversationRepository.SaveChangesAsync();


                    // Notify all admins about the escalated conversation.
                    await _notificationService.NotifyEscalationAsync(conversation.Id, conversation.Title); 
                }

                aiMessage = new Message
                {
                    ConversationId = conversationId,
                    SenderType = SenderType.AI,
                    SenderId = null,
                    Content = aiResponse.Reply
                };

                await _messageRepository.CreateAsync(aiMessage);    
            }


            var messageDto = _mapper.Map<GetMessageDto>(message);

            // Send the user's message to themselves in real time.
            await _chatHubService.SendMessageAsync(userId, messageDto);

            if (!string.IsNullOrWhiteSpace(conversation.AssignedAgentId))
            {
                // Send the user's message to the other participant in real time.

                string receiverId = userId == conversation.CustomerId
                    ? conversation.AssignedAgentId
                    : conversation.CustomerId;

                await _chatHubService.SendMessageAsync(receiverId, messageDto);


                // Notify the other participant about the new message.
                var notification = new SendNotificationDto
                {
                    ConversationId = conversation.Id,
                    ReceiverId = receiverId,
                    Message = $"You have received a new message in conversation \"{conversation.Title}\"."
                };

                await _notificationService.SendAsync(notification);
            }

            if (aiMessage != null)
            {
                // Send the AI's response to the user in real time.
                var aiMessageDto = _mapper.Map<GetMessageDto>(aiMessage);
                await _chatHubService.SendMessageAsync(userId, aiMessageDto);
            }

            return messageDto;
        }


        private async Task<AIResponseDto> GetAIResponseAsync(
            int conversationId, string userId, Message newMessage, bool isFirstMessage)
        {
            var customerMessages = await _messageRepository.GetByConversationIdAsync(conversationId);

            var previousIntent = customerMessages
                .Where(m =>
                    m.Id != newMessage.Id &&
                    m.SenderType == SenderType.Customer &&
                    m.Intent != null)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.Intent)
                .FirstOrDefault();

            var history = _mapper.Map<List<AIMessageDto>>(customerMessages);

            var aiResponse = await _aiService.GenerateResponseAsync(
                history, newMessage.Content, isFirstMessage, previousIntent?.Name);

 
            await AssignIntentToMessageAsync(aiResponse, previousIntent, newMessage);

            return aiResponse;
        }



        // Assigns an existing intent, creates a new intent, or leaves the message
        // without an intent based on the AI's classification.
        private async Task AssignIntentToMessageAsync(
            AIResponseDto aiResponse, Intent? previousIntent, Message newMessage)
        {
            switch (aiResponse.IntentDecision)
            {
                case IntentDecision.SameIntent:

                    if (previousIntent == null)
                    {
                        throw new InvalidOperationException(
                            "The AI selected SameIntent, but no previous intent exists.");
                    }

                    newMessage.IntentId = previousIntent.Id;

                    break;

                case IntentDecision.NewIntent:

                    if (string.IsNullOrWhiteSpace(aiResponse.IntentName) ||
                        string.IsNullOrWhiteSpace(aiResponse.IntentDescription))
                    {
                        throw new InvalidOperationException(
                            "The AI returned NewIntent without a valid name and description.");
                    }

                    newMessage.Intent = new Intent
                    {
                        Name = aiResponse.IntentName,
                        Description = aiResponse.IntentDescription
                    };          

                    break;

                case IntentDecision.NoIntent:
                    break;


                default:
                    throw new InvalidOperationException(
                        $"Unsupported intent decision: {aiResponse.IntentDecision}");
            }

            await _messageRepository.SaveChangesAsync();
        }


        // Gets a conversation or throws NotFoundException if it doesn't exist.
        private async Task<Conversation> GetConversationOrThrowAsync(int id)
        {
            var conversation = await _conversationRepository.GetByIdAsync(id);

            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), id);
            }

            return conversation;
        }



        private void EnsureConversationAccess(Conversation conversation, string userId, string errorMessage)
        {
            if (conversation.CustomerId != userId && conversation.AssignedAgentId != userId)
            {
                throw new ForbiddenException(errorMessage);
            }
        }
    }
}
