using AiCustomerSupport.Application.DTOs.Message;
using AiCustomerSupport.Domain;
using AutoMapper;

namespace AiCustomerSupport.Application.MappingProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, GetMessageDto>()
                .ForMember(
                    dest => dest.CustomerId,
                    opt => opt.MapFrom(src => src.Conversation.CustomerId)
                );
            CreateMap<Message, AIMessageDto>();
        }
    }
}
