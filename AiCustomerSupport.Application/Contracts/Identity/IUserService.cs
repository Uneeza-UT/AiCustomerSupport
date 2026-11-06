using AiCustomerSupport.Application.DTOs.Identity;

namespace AiCustomerSupport.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<List<GetUserInfo>> GetByIdsAsync(List<string> userIds, int pageNumber = 1, int pageSize = 10);
    }
}
