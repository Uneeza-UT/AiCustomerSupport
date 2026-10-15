using AiCustomerSupport.Domain.Common;
using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Domain
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public int? IntentId { get; set; }
        public Intent? Intent { get; set; }

        public SenderType SenderType { get; set; }
        public string? SenderId { get; set; }
        public string Content { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
