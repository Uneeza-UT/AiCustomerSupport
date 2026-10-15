using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Notification;
using AiCustomerSupport.Application.Exceptions;
using AiCustomerSupport.Application.Validations.Notification;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Identity.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatHubService _chatHubService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(IMapper mapper,
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            IChatHubService chatHubService,
            UserManager<ApplicationUser> userManager)
        {
            this._mapper = mapper;
            this._notificationRepository = notificationRepository;
            this._currentUserService = currentUserService;
            this._chatHubService = chatHubService;
            this._userManager = userManager;
        }



        public async Task<List<GetNotificationDto>> GetAsync()
        {
            string userId = _currentUserService.UserId;

            var notifications = await _notificationRepository.GetAllByReceiverIdAsync(userId);
            var data = _mapper.Map<List<GetNotificationDto>>(notifications);
            return data;
        }



        public async Task<List<GetNotificationDto>> GetUnreadAsync()
        {
            string userId = _currentUserService.UserId;

            var notifications = await _notificationRepository.GetUnreadByReceiverIdAsync(userId);
            var data = _mapper.Map<List<GetNotificationDto>>(notifications);
            return data;
        }



        // Sends notification to a specific user.
        public async Task SendAsync(SendNotificationDto dto)
        {
            var validator = new SendNotificationValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid notification entity. ", validationResult);
            }

            var notification = _mapper.Map<Notification>(dto);
            await _notificationRepository.CreateAsync(notification);


            // Send the new notification to receiver in real time via SignalR.

            var notificationDto = _mapper.Map<GetNotificationDto>(notification);
            await _chatHubService.SendNotificationAsync(notificationDto);
        }


        // Sends a list of notifications to specified users
        public async Task SendManyAsync(List<SendNotificationDto> dtos)
        {
            var validator = new SendNotificationValidator();

            foreach (var dto in dtos)
            {
                var validationResult = await validator.ValidateAsync(dto);

                if (validationResult.Errors.Any())
                {
                    throw new BadRequestException("Invalid notification entity. ", validationResult);
                }
            }


            var notifications = _mapper.Map<List<Notification>>(dtos);
            await _notificationRepository.CreateRangeAsync(notifications);



            // Send the new notifications to receivers in real time via SignalR

            var notificationDtos = _mapper.Map<List<GetNotificationDto>>(notifications);
            await _chatHubService.SendNotificationsAsync(notificationDtos);
        }



        // Sends a list of notifications to specified users.
        public async Task NotifyEscalationAsync(int conversationId, string conversationTitle)
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");

            var notifications = users.Select(user => new Notification
            {
                ConversationId = conversationId,
                ReceiverId = user.Id,
                Message = $"A conversation requires human assistance: {conversationTitle}."
            }).ToList();


            if (notifications.Count == 0)
                return;

            await _notificationRepository.CreateRangeAsync(notifications);


            // Send the new notifications to receivers in real time via SignalR.
            var notificationDtos = _mapper.Map<List<GetNotificationDto>>(notifications);
            await _chatHubService.SendNotificationsAsync(notificationDtos);
        }



        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);

            if (notification == null)
            {
                throw new NotFoundException(nameof(Notification), id);
            }


            if (notification.ReceiverId != _currentUserService.UserId)
            {
                throw new ForbiddenException("You can only mark your own notifications as read.");
            }


            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);

            if (notification == null)
            {
                throw new NotFoundException(nameof(Notification), id);
            }


            // Ensures only the notification receiver can delete the notification.

            if (notification.ReceiverId != _currentUserService.UserId)
            {
                throw new ForbiddenException("You can only delete your own notifications.");
            }


            await _notificationRepository.DeleteAsync(notification);
        }
    }
}
