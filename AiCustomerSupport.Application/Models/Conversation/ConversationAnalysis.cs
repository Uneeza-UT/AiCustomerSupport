namespace AiCustomerSupport.Application.Models.Conversation
{
    public class ConversationAnalysis
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Escalated { get; set; }
        public int Assigned { get; set; }
        public string? MostCommonIntent { get; set; }
    }
}
