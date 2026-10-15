using AiCustomerSupport.Application.Models.Conversation;
using AiCustomerSupport.Domain;
using AiCustomerSupport.Domain.Enums;
using System.Linq.Expressions;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation?> GetByIdWithMessagesAsync(int id);

        Task<IReadOnlyList<Conversation>> GetPagedByUserIdAsync(
            string userId, 
            int pageNumber = 1, 
            int pageSize = 10, 
            bool? isDeleted = null);

        Task<IReadOnlyList<Conversation>> GetPagedByAssignedAgentIdAsync(
            string agentUserId, 
            int pageNumber = 1, 
            int pageSize = 10);

        Task<ConversationAnalysis> GetAnalysisAsync();

        Task<IReadOnlyList<Conversation>> SortConversationsAsync(
            Expression<Func<Conversation, object>> keySelector,
            bool descending,
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted,
            int pageNumber,
            int pageSize);

        Task<IReadOnlyList<Conversation>> SearchByIntentAsync(
            string searchTerm,
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted,
            int pageNumber,
            int pageSize);

        Task<IReadOnlyList<Conversation>> FilterByStatusAsync(
            ConversationStatus status,
            string? customerId,
            string? assignedAgentId,
            bool excludeDeleted,
            int pageNumber,
            int pageSize);

        Task<List<string>> GetCustomerIdsAsync();
        Task DeleteAsync(Conversation entity);
    }

}
