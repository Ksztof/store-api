﻿using FluentValidation;
using Store.API.Shared.DTO.Request.Order;

namespace Store.API.Validation.Orders.CrateOrderDto;

internal sealed class CrateOrder : AbstractValidator<CrateOrderDtoApi>
{
    internal CrateOrder()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
            .Matches("^[A-Z][a-zA-Z]*$").WithMessage("First name must start with a capital letter.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("FirstName must not contain any white spaces.")
            .MaximumLength(30).WithMessage("The field must not exceed 30 characters.");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
            .Matches("^[A-Z][a-zA-Z]*$").WithMessage("Last name must start with a capital letter.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("LastName must not contain any white spaces.")
            .MaximumLength(30).WithMessage("The field must not exceed 30 characters.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.")
            .Matches("^[A-Z]").WithMessage("Street must start with a capital letter.")
            .MaximumLength(40).WithMessage("The field must not exceed 40 characters.");

        RuleFor(x => x.StreetNumber).NotEmpty().WithMessage("Street number is required.")
            .Matches("^[a-zA-Z0-9]{1,6}$").WithMessage("Street number must be a valid number with optional letters.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Street number must not contain any white spaces.")
            .MaximumLength(6).WithMessage("The field must not exceed 6 characters.");

        RuleFor(x => x.HomeNumber).NotEmpty().WithMessage("Home number is required.")
            .Matches("^[a-zA-Z0-9]{1,6}$").WithMessage("Home number must be a valid number with optional letters.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("HomeNumber must not contain any white spaces.")
            .MaximumLength(6).WithMessage("The field must not exceed 6 characters.");

        RuleFor(x => x.PostCode).NotEmpty().WithMessage("Post code is required.")
            .Matches(@"^\d{2}-\d{3}$").WithMessage("Post code must be in the format XX-XXX.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Post code must not contain any white spaces.");

        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.")
            .Matches("^[A-Z]").WithMessage("City must start with a capital letter.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Post code must not contain any white spaces.")
            .MaximumLength(30).WithMessage("The field must not exceed 30 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d{3}-\d{3}-\d{3}$").WithMessage("Phone number must be exactly 9 digits.");
    }
}
