using FluentValidation;
using Store.API.Shared.DTO.Request.StoreUser;

namespace Store.API.Validation.StoreUsers.AuthenticateUserDto
{
    public sealed class AuthenticateUser : AbstractValidator<AuthenticateUserDtoApi>
    {
        public AuthenticateUser()
        {
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
        }
    }
}
