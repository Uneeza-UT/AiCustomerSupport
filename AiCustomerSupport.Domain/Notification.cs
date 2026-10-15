using AiCustomerSupport.Domain.Common;

namespace AiCustomerSupport.Domain
{
    public class Notification : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
