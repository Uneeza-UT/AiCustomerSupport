using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Conversation;
using AiCustomerSupport.Application.DTOs.Identity;
using AiCustomerSupport.Domain.Enums;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IConversationService
    {
        Task<ConversationAnalysisDto> GetAnalysisAsync();
        Task<List<GetConversationDto>> GetByUserAsync(PaginationDto dto, string userId);
        Task<GetConversationDto> GetByIdAsync(int id);
        Task<List<GetUserInfo>> GetCustomersWithConversationsAsync();
        Task CreateAsync(CreateConversationDto dto);
        Task ChangeStatusAsync(int id, ChangeConversationStatusDto dto);
        Task DeleteAsync(int id);
        Task<List<GetConversationDto>> SortAsync(SortDto sortDto, string userId);
        Task<List<GetConversationDto>> SearchByIntentAsync(SearchDto dto, string userId);
        Task<List<GetConversationDto>> FilterByStatusAsync(ConversationStatus status, string userId);
    }

}
