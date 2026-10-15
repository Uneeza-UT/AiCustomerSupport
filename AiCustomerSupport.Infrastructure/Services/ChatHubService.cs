using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Application.DTOs.Notification;
using AiCustomerSupport.Domain.Enums;
using AiCustomerSupport.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<SupportChatHub> _hubContext;

        public ChatHubService(IHubContext<SupportChatHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task SendMessageAsync(string receiverId, GetMessageDto messageDto)
        {
            if (messageDto.SenderType == SenderType.AI)
            {
                await _hubContext.Clients
                    .User(receiverId)
                    .SendAsync("ReceiveMessage", messageDto);

                return;
            }

            await _hubContext.Clients
                .Users(messageDto.SenderId!, receiverId)
                .SendAsync("ReceiveMessage", messageDto);
        }


        public async Task SendNotificationAsync(GetNotificationDto notificationDto)
        {
            await _hubContext.Clients
                .User(notificationDto.ReceiverId)
                .SendAsync("ReceiveNotification", notificationDto);
        }


        public async Task SendNotificationsAsync(List<GetNotificationDto> notificationDtos)
        {
            foreach (var notificationDto in notificationDtos)
            {
                await _hubContext.Clients
                .User(notificationDto.ReceiverId)
                .SendAsync("ReceiveNotification", notificationDto);
            }
        }
    }
}
