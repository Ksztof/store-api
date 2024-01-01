using PerfumeStore.Application.Abstractions.Result.Authentication;

namespace PerfumeStore.Application.Abstractions.Result.Shared
{
    public sealed record Error(string code, string? description = null)
    {
        public static readonly Error None = new(string.Empty);
        public static implicit operator AuthenticationResult(Error error) => AuthenticationResult.Failure(error);
    }
}
