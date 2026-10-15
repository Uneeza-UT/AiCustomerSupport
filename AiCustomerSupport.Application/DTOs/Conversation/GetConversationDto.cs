using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Application.DTOs.Conversation
{
    public class GetConversationDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string Title { get; set; }
        public ConversationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GetMessageDto> Messages { get; set; } = new List<GetMessageDto>();
    }
}
