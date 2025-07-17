
using finance_management.DTOs;
using FluentValidation;

namespace finance_management.Services

{
    public class SplitTransactionRequestValidator : AbstractValidator<SplitTransactionRequest>
    {
        public SplitTransactionRequestValidator()
        {
            RuleFor(x => x.SplitAmount)
                .GreaterThan(0).WithMessage("SplitAmount mora biti veći od 0");
        }
    }
}
