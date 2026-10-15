using AiCustomerSupport.Application.DTOs.Message;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IAIService
    {
        Task<AIResponseDto> GenerateResponseAsync(
            List<AIMessageDto> history,
            string newMessage,
            bool isNewConversation,
            string? previousIntentName, 
            CancellationToken cancellationToken = default);
    }
}
