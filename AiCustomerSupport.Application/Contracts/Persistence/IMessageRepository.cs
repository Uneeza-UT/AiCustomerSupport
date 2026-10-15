using AiCustomerSupport.Domain;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IReadOnlyList<Message>> GetPagedByConversationIdAndUserIdAsync(
            int conversationId, string userId, int pageNumber = 1, int pageSize = 10);

        Task<IReadOnlyList<Message>> GetByConversationIdAsync(int conversationId);
        Task<List<Message>> GetAllByIdsWithAttachmentsAsync(List<int> ids);
    }

}
