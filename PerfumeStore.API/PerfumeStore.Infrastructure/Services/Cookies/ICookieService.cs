using Microsoft.AspNetCore.Http;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Application.Shared.Enums;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public interface ICookieService
    {
        public Result SetCookieWithJwtToken(string jwtToken);
        public Result SetCookieWithRefreshToken(string refreshToken);
        public Result SetExpiredCookie(CookieNames cookieName);
    }
}
