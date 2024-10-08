﻿using FluentValidation;
using Store.API.Shared.DTO.Request.Payments;

namespace Store.API.Validation.Payments.GetClientSecretDto;

internal class GetClientSecret : AbstractValidator<GetClientSecretDtoApi>
{
    internal GetClientSecret()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount value cannot be empty")
            .GreaterThan(0).WithMessage("Amount value must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency value cannot be empty")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("PaymentMethodId can't contain white spaces.")
            .Must(ValidationUtils.BeAValidCurrency).WithMessage("Currency value must be a valid ISO currency code");
    }
}
