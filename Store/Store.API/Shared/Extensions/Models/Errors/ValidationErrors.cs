using Store.Domain.Abstractions;

namespace Store.API.Shared.Extensions.Models.Errors;

internal static class ValidationErrors
{
    internal static Error FormValidationError(string propertyName, string errorMessage) =>
        Error.Validation($"{propertyName}.FormValidationError", $"{errorMessage}");
}
