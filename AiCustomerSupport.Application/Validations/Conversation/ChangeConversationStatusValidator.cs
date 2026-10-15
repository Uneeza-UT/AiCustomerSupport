using AiCustomerSupport.Application.DTOs.Conversation;
using FluentValidation;

namespace AiCustomerSupport.Application.Validations.Conversation
{
    public class ChangeConversationStatusValidator : AbstractValidator<ChangeConversationStatusDto>
    {
        public ChangeConversationStatusValidator()
        {
            RuleFor(c => c.Status)
                .IsInEnum().WithMessage("Allowed values: Active, Escalated, Assigned.");
        }
    }
}
