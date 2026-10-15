using AiCustomerSupport.Application.DTOs.Common;
using FluentValidation;

namespace AiCustomerSupport.Application.Validations.Common
{
    public class SortValidator : AbstractValidator<SortDto>
    {
        public SortValidator()
        {
            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Allowed values: Ascending, Descending, Latest, Oldest");
        }
    }
}
