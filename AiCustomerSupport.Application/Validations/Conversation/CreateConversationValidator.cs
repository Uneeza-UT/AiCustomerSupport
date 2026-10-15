using AiCustomerSupport.Application.DTOs.Conversation;
using FluentValidation;

namespace AiCustomerSupport.Application.Validations.Conversation
{
    public class CreateConversationValidator : AbstractValidator<CreateConversationDto>
    {
        public CreateConversationValidator()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");
        }
    }
}
