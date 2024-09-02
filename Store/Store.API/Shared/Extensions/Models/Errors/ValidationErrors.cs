using Store.Domain.Abstractions;

namespace Store.API.Shared.Extensions.Models.Errors
{
    public static class ValidationErrors
    {
        public static Error FormValidationError(string propertyName, string errorMessage) =>
            Error.Validation($"{propertyName}.FormValidationError", $"{errorMessage}");
    }
}
