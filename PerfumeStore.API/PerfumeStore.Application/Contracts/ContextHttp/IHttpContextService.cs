using Microsoft.AspNetCore.Http;
using PerfumeStore.Application.Shared.Enums;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Contracts.ContextHttp
{
    public interface IHttpContextService
    {
        public Result IsUserAuthenticated();

        public Result<string> GetUserId();

        public Result<string> GetActualProtocol();

        public Result SendCookieWithToken(string token, CookieOptions cookieOptions, CookieNames cookieName);
    }
}