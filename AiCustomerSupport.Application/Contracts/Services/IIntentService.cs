using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Intent;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IIntentService
    {
        Task<List<GetIntentDto>> GetAsync(PaginationDto dto);
        Task<GetIntentDto> GetByIdAsync(int id);
        Task<List<GetIntentDto>> SortAsync(SortDto dto);
        Task<List<GetIntentDto>> SearchAsync(SearchDto dto);
    }

}
