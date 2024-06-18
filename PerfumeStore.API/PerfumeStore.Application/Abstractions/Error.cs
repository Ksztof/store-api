using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Shared;

namespace PerfumeStore.Application.Abstractions
{
    public sealed record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);
        public static readonly Error NoneRaw = new(string.Empty, string.Empty);

        private Error(string code, string? description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            Type = errorType;
        }

        public Error(string code, string? description)
        {
            Code = code;
            Description = description;
        }

        public static implicit operator UserResult(Error error) => UserResult.Failure(error);

        public string Code { get; }
        public string Description { get; }
        public ErrorType? Type { get; }

        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
        public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
        public static Error Authorization(string code, string description) => new(code, description, ErrorType.Authorization);
        public static Error Authentication(string code, string description) => new(code, description, ErrorType.Authentication);
        public static Error Server(string code, string description) => new(code, description, ErrorType.Server);
        public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
    }
}
