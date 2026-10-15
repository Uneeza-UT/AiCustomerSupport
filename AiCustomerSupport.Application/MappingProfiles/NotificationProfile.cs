using AiCustomerSupport.Application.DTOs.Notification;
using AiCustomerSupport.Domain;
using AutoMapper;

namespace AiCustomerSupport.Application.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, GetNotificationDto>();
            CreateMap<SendNotificationDto, Notification>();
        }
    }
}
