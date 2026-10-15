using AiCustomerSupport.Application.Models.Email;

namespace AiCustomerSupport.Application.Contracts.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(EmailMessageData emailMessageData);
    }
}
