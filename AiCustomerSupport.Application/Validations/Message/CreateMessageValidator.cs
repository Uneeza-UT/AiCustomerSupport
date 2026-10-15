using AiCustomerSupport.Application.DTOs.Message;
using FluentValidation;

namespace AiCustomerSupport.Application.Validations.Message
{
    public class CreateMessageValidator : AbstractValidator<CreateMessageDto>
    {
        public CreateMessageValidator()
        {
            RuleFor(m => m)
                .Must(m =>
                    !string.IsNullOrWhiteSpace(m.Content) ||
                     (m.Attachments?.Any() ?? false))
                .WithMessage("A message must contain text or at least one attachment.");

        }
    }
}
