using AiCustomerSupport.Application.Enums;

namespace AiCustomerSupport.Application.DTOs.Message
{
    public class AIResponseDto
    {
        public string Reply { get; set; }

        // SameIntent, NewIntent, or NoIntent
        public IntentDecision IntentDecision { get; set; }

        public string? IntentName { get; set; }
        public string? IntentDescription { get; set; }
    }
}
