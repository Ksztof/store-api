using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PerfumeStore.Application.Contracts.ContextHttp;
using PerfumeStore.Application.Contracts.JwtToken.Models;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Application.Shared.Enums;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextService _contextService;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public CookieService(IHttpContextService contextService, IOptions<JwtOptions> jwtOptions)
        {
            _contextService = contextService;
            _jwtOptions = jwtOptions;
        }

        public Result SetCookieWithJwtToken(string jwtToken)
        {

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.Value.RefreshTokenExpirationInHours),//must be equal to SetCookieWithRefreshToken
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
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.Value.RefreshTokenExpirationInHours),
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
}