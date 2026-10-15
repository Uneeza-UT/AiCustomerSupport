using AiCustomerSupport.Application.Enums;

namespace AiCustomerSupport.Application.DTOs.Message
{
    public class AIResponseDto
    {
        public string Reply { get; set; }
        public string? ConversationTitle { get; set; }
        public bool RequiresHumanAgent { get; set; }
        public IntentDecision IntentDecision { get; set; }
        public string? IntentName { get; set; }
        public string? IntentDescription { get; set; }
    }
}
