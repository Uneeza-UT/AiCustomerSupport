using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AiCustomerSupport.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        // Get all notifications belonging to a particular user
        public async Task<IReadOnlyList<Notification>> GetAllByReceiverIdAsync(string receiverId)
        {
            return await _dbContext.Set<Notification>()
                .AsNoTracking()
                .Where(n => n.ReceiverId == receiverId)
                .ToListAsync();
        }


        // Get all unread notifications belonging to a particular user
        public async Task<IReadOnlyList<Notification>> GetUnreadByReceiverIdAsync(string receiverId)
        {
            return await _dbContext.Set<Notification>()
                .AsNoTracking()
                .Where(n => n.ReceiverId == receiverId && !n.IsRead)
                .ToListAsync();
        }


        // Creates multiple notification entities in database
        public async Task CreateRangeAsync(IEnumerable<Notification> notifications)
        {
            await _dbContext.Set<Notification>().AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();
        }
    }
}
