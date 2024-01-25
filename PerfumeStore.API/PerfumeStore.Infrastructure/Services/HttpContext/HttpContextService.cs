using Microsoft.AspNetCore.Http;
using PerfumeStore.Application.Contracts.HttpContext;
using System.Security.Claims;

namespace PerfumeStore.Infrastructure.Services.HttpContext
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserAuthenticated()
        {
            bool? userIsAuthenticated = _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;

            if (userIsAuthenticated == null)
            {
                return false;
            }

            return userIsAuthenticated.Value;
        }

        public string GetUserId()
        {
            string? userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId;
        }

        public string GetActualProtocol()
        {
            string? protocol = _httpContextAccessor?.HttpContext?.Request.Scheme;

            return protocol;
        }

        public void SendCookieWithToken(string token, CookieOptions cookieOptions)
        {
            string cookieName = "AuthToken";

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(cookieName, token, cookieOptions);
        }
    }
}
