using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Application.DTOs.Message
{
    public class AIMessageDto
    {
        public SenderType SenderType { get; set; }
        public string Content { get; set; }
    }
}
