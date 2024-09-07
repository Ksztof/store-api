using FluentValidation;
using FluentValidation.Results;

namespace Store.API.Validation;

internal class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    internal ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResult> ValidateAsync<T>(T instance)
    {
        var validator = (IValidator<T>)_serviceProvider.GetService(typeof(IValidator<T>));

        if (validator != null)
        {
            var context = new ValidationContext<T>(instance);
            return await validator.ValidateAsync(context);
        }
        else
        {
            return new ValidationResult();
        }
    }
}
