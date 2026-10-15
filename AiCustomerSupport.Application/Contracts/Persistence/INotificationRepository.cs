using AiCustomerSupport.Domain;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetAllByReceiverIdAsync(string receiverId);
        Task<IReadOnlyList<Notification>> GetUnreadByReceiverIdAsync(string receiverId);
        Task CreateRangeAsync(IEnumerable<Notification> notifications);
    }
}
