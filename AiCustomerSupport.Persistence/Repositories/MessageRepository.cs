using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AiCustomerSupport.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }


        public async Task<IReadOnlyList<Message>> GetPagedByConversationIdAndUserIdAsync(
            int conversationId, string userId, int pageNumber = 1, int pageSize = 10)
        {
            int skip = (pageNumber - 1) * pageSize;

            return await _dbContext.Set<Message>()
                .AsNoTracking()   
                .Where(m => m.ConversationId == conversationId &&
                    (m.Conversation.CustomerId == userId || m.Conversation.AssignedAgentId == userId))
                .OrderBy(m => m.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Include(m => m.Attachments)
                .ToListAsync();
        }




        public async Task<IReadOnlyList<Message>> GetByConversationIdAsync(int conversationId)
        {
            return await _dbContext.Set<Message>()
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .Include(m => m.Intent)
                .ToListAsync();
        }



        // Get a list of message entities with attachments
        public async Task<List<Message>> GetAllByIdsWithAttachmentsAsync(List<int> ids)
        {
            return await _dbContext.Set<Message>()
                .AsNoTracking()  
                .Where(m => ids.Contains(m.Id))
                .Include(m => m.Attachments)
                .ToListAsync();
        }
    }
}
