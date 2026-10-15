using AiCustomerSupport.Application.DTOs.Message;

namespace AiCustomerSupport.Application.DTOs.Intent
{
    public class GetIntentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<GetMessageDto> Messages { get; set; } = new List<GetMessageDto>();
    }
}
