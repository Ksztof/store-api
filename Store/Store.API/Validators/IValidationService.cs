
using FluentValidation.Results;

namespace Store.API.Validators
{
    public interface IValidationService
    {
        public Task<ValidationResult> ValidateAsync<T>(T instance);
    }
}