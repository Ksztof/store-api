namespace Store.Application.Contracts.JwtToken.Models;

public class JwtOptions
{
    public string ValidIssuer { get; init; } = string.Empty;
    public string ValidAudience { get; init; } = string.Empty;
    public double JwtTokenExpirationInHours { get; init; }
    public double JwtCookieExpirationInHours { get; init; }
    public double RefreshTokenExpirationInHours { get; init; }
}
