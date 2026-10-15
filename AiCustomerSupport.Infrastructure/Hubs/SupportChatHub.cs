using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AiCustomerSupport.Infrastructure.Hubs
{
    [Authorize]
    public class SupportChatHub : Hub
    {

    }
}
