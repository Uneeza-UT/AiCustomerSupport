using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Conversation;
using AiCustomerSupport.Application.DTOs.Identity;
using AiCustomerSupport.Application.Enums;
using AiCustomerSupport.Application.Exceptions;
using AiCustomerSupport.Application.Validations.Common;
using AiCustomerSupport.Application.Validations.Conversation;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Domain.Enums;
using AiCustomerSupport.Identity.Services;
using AutoMapper;
using System.Linq.Expressions;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IMapper _mapper;
        private readonly IConversationRepository _conversationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserService _userService;

        public ConversationService(IMapper mapper,
            IConversationRepository conversationRepository,
            ICurrentUserService currentUserService,
            UserService userService)
        {
            _mapper = mapper;
            _conversationRepository = conversationRepository;
            _currentUserService = currentUserService;
            _userService = userService;
        }


        public async Task<ConversationAnalysisDto> GetAnalysisAsync()
        {
            var analysis = await _conversationRepository.GetAnalysisAsync();
            return _mapper.Map<ConversationAnalysisDto>(analysis);
        }



        public async Task<List<GetConversationDto>> GetByUserAsync(PaginationDto dto, string userId)
        {
            string role = _currentUserService.UserRole;
            string currentUserId = _currentUserService.UserId;

            IReadOnlyList<Conversation> conversations = role switch
            {
                "Admin" => await _conversationRepository
                    .GetPagedByUserIdAsync(userId, dto.PageNumber, dto.PageSize, true),


                "Agent" => await _conversationRepository
                    .GetPagedByAssignedAgentIdAsync(currentUserId, dto.PageNumber, dto.PageSize),


                "Customer" when userId == currentUserId =>
                    await _conversationRepository.GetPagedByUserIdAsync(
                        userId, dto.PageNumber, dto.PageSize),

                "Customer" => throw new ForbiddenException("You can only view your own conversations."),

                _ => throw new ForbiddenException("You are not authorized to view conversations.")
            };      

            return _mapper.Map<List<GetConversationDto>>(conversations);
        }



        public async Task<GetConversationDto> GetByIdAsync(int id)
        {
            string role = _currentUserService.UserRole;
            string userId = _currentUserService.UserId;

            var conversation = await _conversationRepository.GetByIdWithMessagesAsync(id);

            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), id);
            }


            bool isAuthorized = role switch
            {
                "Admin" => true,

                "Agent" => userId == conversation.AssignedAgentId,

                "Customer" => userId == conversation.CustomerId,

                _ => false
            };


            if (!isAuthorized)
            {
                throw new ForbiddenException("You are not authorized to view this conversation.");
            }

           return _mapper.Map<GetConversationDto>(conversation);
        }



        public async Task<List<GetUserInfo>> GetCustomersWithConversationsAsync()
        {
            var customerIds = await _conversationRepository.GetCustomerIdsAsync();
            var customers = await _userService.GetByIdsAsync(customerIds);
            return customers;      
        }



        public async Task CreateAsync(CreateConversationDto dto)
        {
            // Validate the dto.
            var validator = new CreateConversationValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid conversation entity. ", validationResult);
            }

            var conversation = _mapper.Map<Conversation>(dto);
            await _conversationRepository.CreateAsync(conversation);
        }


        
        public async Task AssignToAgentAsync(int id, AssignConversationToAgentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AgentId))
            {
                throw new BadRequestException("Invalid agent Id.");
            }


            var conversation = await _conversationRepository.GetByIdAsync(id);

            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), id);
            }

            conversation.AssignedAgentId = dto.AgentId;
            conversation.Status = ConversationStatus.Assigned;
            await _conversationRepository.SaveChangesAsync();
        }



        // Only Admins and Customers are authorized to perform this task.
        public async Task DeleteAsync(int id)
        {
            var role = _currentUserService.UserRole;
            var conversation = await _conversationRepository.GetByIdAsync(id);

            if (conversation == null)
            {
                throw new NotFoundException(nameof(Conversation), id);
            }

            if (role == "Admin")
            {
                conversation.IsDeleted = true;
                await _conversationRepository.SaveChangesAsync();
            }

            else if (role == "Customer")
            {
                // Ensure customers can only delete conversations they own.
                if (conversation.CustomerId != _currentUserService.UserId)
                {
                    throw new ForbiddenException("You can only delete your own conversations.");
                }

                await _conversationRepository.DeleteAsync(conversation);
            }

            else
            {
                throw new ForbiddenException("You are not authorized to delete conversations.");
            }    
        }



        public async Task<List<GetConversationDto>> SortAsync(SortDto sortDto, string userId)
        {
            // Validate the dto.
            var validator = new SortValidator();
            var validationResult = await validator.ValidateAsync(sortDto);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid sort option. ", validationResult);
            }


            var (customerId, assignedAgentId, excludeDeleted) = GetConversationAuthorizationScope(userId);

            bool descending;
            Expression<Func<Conversation, object>> sortExpression;

            switch (sortDto.SortBy)
            {
                case SortOption.Ascending:
                    descending = false;
                    sortExpression = c => c.Title;
                    break;

                case SortOption.Descending:
                    descending = true;
                    sortExpression = c => c.Title;
                    break;

                case SortOption.Latest:
                    descending = true;
                    sortExpression = c => c.CreatedAt;
                    break;

                case SortOption.Oldest:
                    descending = false;
                    sortExpression = c => c.CreatedAt;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(sortDto.SortBy),
                        sortDto.SortBy,
                        "Invalid sort option.");
            }

            var paginationDto = new PaginationDto();

            var conversations = await _conversationRepository
                .SortConversationsAsync(
                    sortExpression,
                    descending,
                    customerId,
                    assignedAgentId,
                    excludeDeleted,         
                    paginationDto.PageNumber,
                    paginationDto.PageSize);

            return _mapper.Map<List<GetConversationDto>>(conversations);
        }



        public async Task<List<GetConversationDto>> SearchByIntentAsync(SearchDto dto, string userId)
        {
            string searchTerm = dto.SearchTerm?.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetByUserAsync(new PaginationDto(), userId);
            }

            var (customerId, assignedAgentId, excludeDeleted) = GetConversationAuthorizationScope(userId);

            var paginationDto = new PaginationDto();

            var conversations = await _conversationRepository
                .SearchByIntentAsync(
                    searchTerm,
                    customerId,
                    assignedAgentId,
                    excludeDeleted,
                    paginationDto.PageNumber,
                    paginationDto.PageSize);

            return _mapper.Map<List<GetConversationDto>>(conversations);
        }



        public async Task<List<GetConversationDto>> FilterByStatusAsync(
            ConversationStatus status, string userId)
        {
            if (!Enum.IsDefined(typeof(ConversationStatus), status))
            {
                throw new BadRequestException("Invalid conversation status.");
            }

            var (customerId, assignedAgentId, excludeDeleted) = GetConversationAuthorizationScope(userId);

            var paginationDto = new PaginationDto();

            var conversations = await _conversationRepository
                .FilterByStatusAsync(
                    status,
                    customerId,
                    assignedAgentId,
                    excludeDeleted,
                    paginationDto.PageNumber,
                    paginationDto.PageSize);

            return _mapper.Map<List<GetConversationDto>>(conversations);
        }



        // Determines the conversation access scope based on the current user's role and the specified user Id.
        private (string? CustomerId, string? AssignedAgentId, bool excludeDeleted) 
            GetConversationAuthorizationScope(string userId)
        {
            string role = _currentUserService.UserRole;
            string currentUserId = _currentUserService.UserId;

            switch (role)
            {
                case "Admin":
                    return (userId, null, true);

                case "Agent":
                    return (null, currentUserId, true);

                case "Customer" when userId == currentUserId:
                    return (currentUserId, null, false);

                case "Customer":
                    throw new ForbiddenException(
                        "You can only view your own conversations.");

                default:
                    throw new ForbiddenException(
                        "You are not authorized to view conversations.");
            }
        }
    }
}
