﻿using FluentValidation;

namespace Store.API.Validation.Orders.CrateOrderDto;

internal sealed class CrateOrderMethod : AbstractValidator<string>
{
    internal CrateOrderMethod()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Method is required.")
        .Matches("^[a-zA-Z]+$").WithMessage("Method must be a valid string.")
        .Must(ValidationUtils.BeAValidOrderMethod).WithMessage("Method must be a valid order method.");
    }
}
