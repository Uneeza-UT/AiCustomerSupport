using AiCustomerSupport.Application.DTOs.Attachment;
using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Application.DTOs.Message
{
    public class GetMessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public SenderType SenderType { get; set; }
        public string? SenderId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GetAttachmentDto> Attachments { get; set; } = new List<GetAttachmentDto>();
    }
}
