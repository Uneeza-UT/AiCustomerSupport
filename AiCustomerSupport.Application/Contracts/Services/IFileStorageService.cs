using Microsoft.AspNetCore.Http;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
