using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Message;

namespace AiCustomerSupport.Application.Contracts.Services
{
    public interface IMessageService
    {
        Task<List<GetMessageDto>> GetAsync(int conversationId, PaginationDto dto);
        Task<GetMessageDto> SendAsync(int conversationId, CreateMessageDto dto);
    }
}
