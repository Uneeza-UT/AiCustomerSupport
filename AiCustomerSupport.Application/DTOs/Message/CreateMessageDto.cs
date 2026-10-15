using Microsoft.AspNetCore.Http;

namespace AiCustomerSupport.Application.DTOs.Message
{
    public class CreateMessageDto
    {
        public string? Content { get; set; }
        public List<IFormFile> Attachments { get; set; } = new();
    }
}
