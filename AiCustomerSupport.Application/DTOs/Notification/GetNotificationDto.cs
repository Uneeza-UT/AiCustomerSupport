namespace AiCustomerSupport.Application.DTOs.Notification
{
    public class GetNotificationDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
