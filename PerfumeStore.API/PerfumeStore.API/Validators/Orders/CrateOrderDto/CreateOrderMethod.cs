using FluentValidation;

namespace PerfumeStore.API.Validators.Orders.CrateOrderDto
{
    public sealed class CrateOrderMethod : AbstractValidator<string>
    {
        public CrateOrderMethod()
        {
            RuleFor(x => x).NotEmpty().WithMessage("Method is required.")
            .Matches("^[a-zA-Z]+$").WithMessage("Method must be a valid string.")
            .Must(ValidationUtils.BeAValidOrderMethod).WithMessage("Method must be a valid order method.");
        }
    }
}
