using AiCustomerSupport.Application.DTOs.Notification;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface INotificationService
    {
        Task<List<GetNotificationDto>> GetAsync();
        Task<List<GetNotificationDto>> GetUnreadAsync();
        Task SendAsync(SendNotificationDto dto);
        Task SendManyAsync(List<SendNotificationDto> dtos);
        Task NotifyEscalationAsync(int conversationId, string conversationTitle);
        Task MarkAsReadAsync(int id);
        Task DeleteAsync(int id);
    }
}
