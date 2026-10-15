using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.DTOs.Identity;
using AiCustomerSupport.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AiCustomerSupport.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<GetUserInfo>> GetByIdsAsync(List<string> userIds)
        {
            return await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new GetUserInfo
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailAddress = u.Email
                })
                .ToListAsync();
        }
    }
}
