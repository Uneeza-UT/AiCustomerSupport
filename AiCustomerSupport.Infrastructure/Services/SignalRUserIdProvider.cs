using Microsoft.AspNetCore.SignalR;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value;
        }
    }
}
