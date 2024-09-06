using FluentValidation;
using Store.API.Shared.DTO.Request.StoreUser;

namespace Store.API.Validation.StoreUsers.RegisterUserDto;

internal sealed class RegisterUser : AbstractValidator<RegisterUserDtoApi>
{
    internal RegisterUser()
    {
        RuleFor(x => x.Login)
       .NotEmpty().WithMessage("Login is required.")
       .MinimumLength(2).WithMessage("Login must be at least 2 characters long.")
       .MaximumLength(15).WithMessage("Login can have a maximum of 15 characters.")
       .Matches(@"^[a-zA-Z][a-zA-Z0-9]*$").WithMessage("Login must start with a letter and contain only letters and digits.")
       .Must(ValidationUtils.HaveAtLeastOneLetter).WithMessage("Login must contain at least one letter.")
       .Must(ValidationUtils.StartWithLetter).WithMessage("Login must start with a letter.")
       .Must(ValidationUtils.NotContainSpecialCharactersOrWhiteSpace).WithMessage("Login cannot contain special characters or whitespace.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(7).WithMessage("Password must be at least 7 characters long.")
        .Must(ValidationUtils.HaveMinimumNumberOfLetters).WithMessage("Password must contain at least 4 letters.")
        .Must(ValidationUtils.HaveMinimumNumberOfDigits).WithMessage("Password must contain at least 2 digits.")
        .Must(ValidationUtils.HaveAtLeastOneSpecialCharacter).WithMessage("Password must contain at least one special character.")
        .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Password must not contain any white spaces.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Confirm password must match password.")
            .MinimumLength(7).WithMessage("Confirm password must be at least 7 characters long.")
            .Must(ValidationUtils.HaveMinimumNumberOfLetters).WithMessage("Confirm password must contain at least 4 letters.")
            .Must(ValidationUtils.HaveMinimumNumberOfDigits).WithMessage("Confirm password must contain at least 2 digits.")
            .Must(ValidationUtils.HaveAtLeastOneSpecialCharacter).WithMessage("Confirm password must contain at least one special character.")
            .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Confirm password must not contain any white spaces.");
    }
}
