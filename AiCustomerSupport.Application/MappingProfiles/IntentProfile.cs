using AiCustomerSupport.Application.DTOs.Intent;
using AiCustomerSupport.Domain;
using AutoMapper;

namespace AiCustomerSupport.Application.MappingProfiles
{
    public class IntentProfile : Profile
    {
        public IntentProfile()
        {
            CreateMap<Intent, GetIntentDto>();
        }
    }
}
