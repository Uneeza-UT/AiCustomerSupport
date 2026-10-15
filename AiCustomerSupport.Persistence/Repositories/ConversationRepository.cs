using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Models.Conversation;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Domain.Enums;
using AiCustomerSupport.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AiCustomerSupport.Persistence.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(int id)
        {
            return await _dbContext.Set<Conversation>()
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        // Fetch paginated conversations belonging to a specific customer.
        public async Task<IReadOnlyList<Conversation>> GetPagedByUserIdAsync(
            string userId, 
            int pageNumber = 1, 
            int pageSize = 10, 
            bool? isDeleted = null)
        {
            int skip = (pageNumber - 1) * pageSize;

            return await _dbContext.Set<Conversation>()
                .AsNoTracking()            
                .Where(c => c.CustomerId == userId && c.IsDeleted != isDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Include(c => c.Messages)
                .ToListAsync();
        }



        // Fetch paginated conversations assigned to a specific agent.
        public async Task<IReadOnlyList<Conversation>> GetPagedByAssignedAgentIdAsync(
            string agentUserId, 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            int skip = (pageNumber - 1) * pageSize;

            return await _dbContext.Set<Conversation>()
                .AsNoTracking()
                .Where(c => c.AssignedAgentId == agentUserId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Include(c => c.Messages)
                .ToListAsync();
        }


        // Get conversation analysis.
        public async Task<ConversationAnalysis> GetAnalysisAsync()
        {
            return new ConversationAnalysis
            {
                Total = await _dbContext.Set<Conversation>().CountAsync(),

                Active = await _dbContext.Set<Conversation>()
                    .CountAsync(c => c.Status == ConversationStatus.Active),

                Escalated = await _dbContext.Set<Conversation>()
                    .CountAsync(c => c.Status == ConversationStatus.Escalated),

                Assigned = await _dbContext.Set<Conversation>()
                    .CountAsync(c => c.Status == ConversationStatus.Assigned),

                MostCommonIntent = await _dbContext.Set<Message>()
                    .Where(m => m.Intent != null)
                    .GroupBy(m => new { m.IntentId, m.Intent!.Name })
                    .Select(g => new
                    {
                        IntentName = g.Key.Name,
                        ConversationCount = g.Select(m => m.ConversationId)
                            .Distinct()
                            .Count()
                    })
                    .OrderByDescending(c => c.ConversationCount)
                    .ThenBy(c => c.IntentName)
                    .Select(c => c.IntentName)
                    .FirstOrDefaultAsync()
            };
        }



        public async Task<IReadOnlyList<Conversation>> SortConversationsAsync(
            Expression<Func<Conversation, object>> keySelector,
            bool descending,
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted,  
            int pageNumber,
            int pageSize)
        {
            var query = GetConversationQuery(customerId, assignedAgentId, excludeDeleted);

            query = descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }



        public async Task<IReadOnlyList<Conversation>> SearchByIntentAsync(
            string searchTerm, 
            string? customerId, 
            string? assignedAgentId,
            bool excludeDeleted,
            int pageNumber,
            int pageSize)
        {
            var query = GetConversationQuery(customerId, assignedAgentId, excludeDeleted);

            return await _dbContext.Set<Conversation>()
                .Where(c => c.Messages.Any(m => 
                    m.Intent != null &&
                    m.Intent.Name.Contains(searchTerm)))
                .Include(c => c.Messages)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<IReadOnlyList<Conversation>> FilterByStatusAsync(
            ConversationStatus status,
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted,
            int pageNumber,
            int pageSize)
        {
            var query = GetConversationQuery(customerId, assignedAgentId, excludeDeleted);

            return await query
                .Where(c => c.Status == status)
                .Include(c => c.Messages)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<List<string>> GetCustomerIdsAsync()
        {
            return await _dbContext.Set<Conversation>()
                .AsNoTracking()
                .Select(c => c.CustomerId)
                .Distinct()
                .ToListAsync();
        }


        // Builds a conversation query with optional customer, agent, and soft-delete filters.
        private IQueryable<Conversation> GetConversationQuery(
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted = true)
        {
            var query = _dbContext.Set<Conversation>()
                .AsNoTracking();


            // Exclude soft-deleted conversations when excludeDeleted is true.
            if (excludeDeleted)
                query = query.Where(c => !c.IsDeleted);


            if (customerId != null)
                query = query.Where(c => c.CustomerId == customerId);

            if (assignedAgentId != null)
                query = query.Where(c => c.AssignedAgentId == assignedAgentId);

            return query;
        }



        public async Task DeleteAsync(Conversation entity)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
