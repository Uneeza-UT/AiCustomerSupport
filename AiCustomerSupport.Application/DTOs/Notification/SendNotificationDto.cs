namespace AiCustomerSupport.Application.DTOs.Notification
{
    public class SendNotificationDto
    {
        public int ConversationId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
    }
}
