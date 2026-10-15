using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Application.DTOs.Notification;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IChatHubService
    {
        Task SendMessageAsync(string receiverId, GetMessageDto messageDto);
        Task SendNotificationAsync(GetNotificationDto notificationDto);
        Task SendNotificationsAsync(List<GetNotificationDto> notificationDtos);
    }
}
