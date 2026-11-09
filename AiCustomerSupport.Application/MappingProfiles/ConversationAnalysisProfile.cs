using AiCustomerSupport.Application.DTOs.Conversation;
using AiCustomerSupport.Application.Models.Conversation;
using AutoMapper;

namespace AiCustomerSupport.Application.MappingProfiles
{
    public class ConversationAnalysisProfile : Profile
    {
        public ConversationAnalysisProfile()
        {
            CreateMap<ConversationAnalysis, ConversationAnalysisDto>();
        }
    }
}
