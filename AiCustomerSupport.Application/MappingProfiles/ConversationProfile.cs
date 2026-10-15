using AiCustomerSupport.Application.DTOs.Conversation;
using AiCustomerSupport.Application.Models.Conversation;
using AiCustomerSupport.Domain;
using AutoMapper;

namespace AiCustomerSupport.Application.MappingProfiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, GetConversationDto>();
            CreateMap<ConversationAnalysisDto, ConversationAnalysis>();
        }
    }
}
