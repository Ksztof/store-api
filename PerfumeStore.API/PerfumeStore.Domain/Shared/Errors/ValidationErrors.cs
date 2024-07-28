using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Domain.Shared.Errors
{
    public static class ValidationErrors
    {
        public static Error FormValidationError(string propertyName, string errorMessage) =>
            Error.Validation($"{propertyName}.FormValidationError", $"{errorMessage}");
    }
}
