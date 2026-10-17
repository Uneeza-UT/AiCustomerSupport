using AiCustomerSupport.Domain;

namespace AiCustomerSupport.Application.Contracts.Persistence
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IReadOnlyList<Message>> GetByConversationIdAndUserIdAsync(int conversationId, string userId);
        Task<IReadOnlyList<Message>> GetByConversationIdAsync(int conversationId);
    }

}
