using FluentValidation;

namespace PerfumeStore.Application.Validators
{
    public class QuantityValidator : AbstractValidator<decimal>
    {
        public QuantityValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                    .WithMessage("Value can't be empty")
                .GreaterThan(0)
                    .WithMessage("Value can't be 0 or negative")
                .LessThan(1000)
                    .WithMessage("Value must be less than 1000.")
                .Must(value => value * 100 % 1 == 0)
                    .WithMessage("Value can have up to two decimal places.");
        }
    }
}
