using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.JwtToken.Models;
using Store.Application.Users.Enums;
using Store.Domain.Abstractions;

namespace Store.Infrastructure.Services.Cookies;

public class CookieService : ICookieService
{
    private readonly IHttpContextService _contextService;
    private readonly JwtOptions _jwtOptions;

    public CookieService(
        IHttpContextService contextService,
        IOptions<JwtOptions> jwtOptions)
    {
        _contextService = contextService;
        _jwtOptions = jwtOptions.Value;
    }

    public Result SetCookieWithJwtToken(string jwtToken)
    {

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpirationInHours),
            IsEssential = false,
            SameSite = SameSiteMode.None,
        };

        Result result = _contextService.SendCookieWithToken(jwtToken, cookieOptions, CookieNames.AuthCookie);

        if (result.IsFailure)
        {
            return result;
        }

        return result;
    }

    public Result SetCookieWithRefreshToken(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpirationInHours),
            IsEssential = false,
            SameSite = SameSiteMode.None,
        };

        Result result = _contextService.SendCookieWithToken(refreshToken, cookieOptions, CookieNames.RefreshCookie);

        if (result.IsFailure)
        {
            return result;
        }

        return Result.Success();
    }

    public Result SetExpiredCookie(CookieNames cookieName)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(-1),
            IsEssential = false,
            SameSite = SameSiteMode.None,

        };

        Result result = _contextService.SendCookieWithToken(string.Empty, cookieOptions, cookieName);

        if (result.IsFailure)
        {
            return result;
        }

        return result;
    }
}