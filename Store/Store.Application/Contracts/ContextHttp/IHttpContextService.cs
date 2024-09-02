using Microsoft.AspNetCore.Http;
using Store.Application.Users.Enums;
using Store.Domain.Abstractions;

namespace Store.Application.Contracts.ContextHttp
{
    public interface IHttpContextService
    {
        public Result IsUserAuthenticated();

        public Result<string> GetUserId();

        public Result<string> GetActualProtocol();

        public Result SendCookieWithToken(string token, CookieOptions cookieOptions, CookieNames cookieName);
    }
}