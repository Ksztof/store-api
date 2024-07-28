using Microsoft.AspNetCore.Http;
using PerfumeStore.Application.Contracts.ContextHttp;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextService _contextService;

        public CookieService(IHttpContextService contextService)
        {
            _contextService = contextService;
        }

        public Result SetCookieWithToken(string token)
        {
            var time = DateTimeOffset.Now.AddDays(1);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddDays(1),
                IsEssential = false,
                SameSite = SameSiteMode.None,
            };

            Result result = _contextService.SendCookieWithToken(token, cookieOptions);

            return result;  
        }

        public Result SetExpiredAuthToken(CookieOptions options)
        {
            Result result = _contextService.SendCookieWithToken(string.Empty, options);

            return result;
        }
    }
}
