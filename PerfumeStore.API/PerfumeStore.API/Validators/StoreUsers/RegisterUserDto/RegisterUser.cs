using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.StoreUser;

namespace PerfumeStore.API.Validators.StoreUsers.RegisterUserDto
{
    public sealed class RegisterUser : AbstractValidator<RegisterUserDtoApi>
    {
        public RegisterUser() 
        {
            RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .MinimumLength(2).WithMessage("Login must be at least 2 characters long.")
            .Matches(@"^[A-Z][a-zA-Z]+$").WithMessage("Login must start with a capital letter and contain only letters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(7).WithMessage("Password must be at least 7 characters long.")
                .Must(ValidationUtils.HaveMinimumNumberOfLetters).WithMessage("Password must contain at least 4 letters.")
                .Must(ValidationUtils.HaveMinimumNumberOfDigits).WithMessage("Password must contain at least 2 digits.")
                .Must(ValidationUtils.HaveAtLeastOneSpecialCharacter).WithMessage("Password must contain at least one special character.")
                .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Password must not contain any white spaces.")
                .Equal(x => x.Password).WithMessage("Confirm password must match password.");


            RuleFor(x => x.ConfirmPassword)
               .NotEmpty().WithMessage("Password is required.")
               .MinimumLength(7).WithMessage("Password must be at least 7 characters long.")
               .Must(ValidationUtils.HaveMinimumNumberOfLetters).WithMessage("Password must contain at least 4 letters.")
               .Must(ValidationUtils.HaveMinimumNumberOfDigits).WithMessage("Password must contain at least 2 digits.")
               .Must(ValidationUtils.HaveAtLeastOneSpecialCharacter).WithMessage("Password must contain at least one special character.")
               .Must(ValidationUtils.NotContainWhiteSpace).WithMessage("Password must not contain any white spaces.");
        }
    }
}
