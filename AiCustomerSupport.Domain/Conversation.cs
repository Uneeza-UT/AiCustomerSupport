using AiCustomerSupport.Domain.Common;
using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Domain
{
    public class Conversation : BaseEntity
    {
        public string CustomerId { get; set; }   
        public string Title { get; set; }   
        public ConversationStatus Status { get; set; }
        public string? AssignedAgentId { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
