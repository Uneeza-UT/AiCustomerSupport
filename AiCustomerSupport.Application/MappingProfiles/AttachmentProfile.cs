using AiCustomerSupport.Application.DTOs.Attachment;
using AiCustomerSupport.Domain;
using AutoMapper;


namespace AiCustomerSupport.Application.MappingProfiles
{
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<Attachment, GetAttachmentDto>();
        }
    }
}
