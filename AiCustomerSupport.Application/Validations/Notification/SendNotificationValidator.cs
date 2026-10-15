using AiCustomerSupport.Application.DTOs.Notification;
using FluentValidation;

namespace AiCustomerSupport.Application.Validations.Notification
{
    public class SendNotificationValidator : AbstractValidator<SendNotificationDto>
    {
        public SendNotificationValidator()
        {
            RuleFor(n => n.ConversationId)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");


            RuleFor(n => n.ReceiverId)
                .NotEmpty().WithMessage("{PropertyName} is required.");


            RuleFor(n => n.Message)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(500).WithMessage("{PropertyName} cannot exceed 500 characters");
        }
    }
}
