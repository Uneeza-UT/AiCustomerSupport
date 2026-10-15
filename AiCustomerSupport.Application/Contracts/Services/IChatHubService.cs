using AiCustomerSupport.Application.DTOs.Message;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IChatHubService
    {
        Task SendMessageAsync(string receiverId, GetMessageDto messageDto);
    }
}
