using FluentValidation.Results;

namespace PerfumeStore.API.Validators
{
    public interface IValidationService
    {
        public Task<ValidationResult> ValidateAsync<T>(T instance);
    }
}