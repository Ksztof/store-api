using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Payments;

namespace PerfumeStore.API.Validators.Payments.PayWithCardDto
{
    public class PayWithCard : AbstractValidator<StartOrderDtoApi>
    {
        public PayWithCard()
        {
            RuleFor(x => x.PaymentMethodId)
                .NotEmpty().WithMessage("PaymentMethodId value cannot be empty")
                .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("PaymentMethodId can't contain white spaces.");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount value cannot be empty")
                .GreaterThan(0).WithMessage("Amount value must be greater than zero");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency value cannot be empty")
                .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("PaymentMethodId can't contain white spaces.")
                .Must(ValidationUtils.BeAValidCurrency).WithMessage("Currency value must be a valid ISO currency code");
        }
    }
}
