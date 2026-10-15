using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Domain.Enums;
using AiCustomerSupport.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<SupportChatHub> _hubContext;

        public ChatHubService(IHubContext<SupportChatHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task SendMessageAsync(string receiverId, GetMessageDto messageDto)
        {
            if (messageDto.SenderType == SenderType.AI)
            {
                await _hubContext.Clients
                    .User(receiverId)
                    .SendAsync("ReceiveMessage", messageDto);

                return;
            }

            await _hubContext.Clients
                .Users(messageDto.SenderId!, receiverId)
                .SendAsync("ReceiveMessage", messageDto);
        }
    }
}
