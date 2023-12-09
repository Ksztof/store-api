namespace PerfumeStore.Application.Core
{
    public class JwtOptions
    {
        public string SecurityKey { get; init; } = string.Empty;
        public string ValidIssuer { get; init; } = string.Empty;
        public string ValidAudience { get; init; } = string.Empty;
        public int ExpiryInMinutes { get; init; }
    }
}
