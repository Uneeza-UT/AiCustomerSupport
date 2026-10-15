using AiCustomerSupport.Application.Contracts.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AiCustomerSupport.Identity.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly HttpContextAccessor _httpContextAccessor;

        public CurrentUserService(HttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId =>
            _httpContextAccessor.HttpContext?
            .User?
            .FindFirst("UserId")?
            .Value;


        public string UserRole =>
            _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.Role)
            .Value;
    }
}
