using AiCustomerSupport.Application.Contracts.Email;
using AiCustomerSupport.Application.Models.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace AiCustomerSupport.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly IResend _resend;
        public EmailSettings _emailSettings { get; }
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IResend resend, IOptions<EmailSettings> emailSettings,
            ILogger<EmailSender> logger)
        {
            _resend = resend;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }


        public async Task<bool> SendEmailAsync(EmailMessageData emailMessageData)
        {
            try
            {
                var message = new EmailMessage
                {
                    From = $"{_emailSettings.FromName} <{_emailSettings.FromAddress}>",
                    To = { emailMessageData.To },
                    Subject = emailMessageData.Subject,
                    TextBody = emailMessageData.Body
                };

                var response = await _resend.EmailSendAsync(message);
                return response.Success;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}.", emailMessageData.To);
                return false;
            }
        }
    }
}
