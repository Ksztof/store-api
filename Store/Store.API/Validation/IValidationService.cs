using FluentValidation.Results;

namespace Store.API.Validation;

public interface IValidationService
{
    public Task<ValidationResult> ValidateAsync<T>(T instance);
}