using Store.Application.Users.Enums;
using Store.Domain.Abstractions;

namespace Store.Infrastructure.Services.Cookies;

public interface ICookieService
{
    public Result SetCookieWithJwtToken(string jwtToken);
    public Result SetCookieWithRefreshToken(string refreshToken);
    public Result SetExpiredCookie(CookieNames cookieName);
}
