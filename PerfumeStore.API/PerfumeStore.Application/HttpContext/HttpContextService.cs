using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PerfumeStore.Application.HttpContext
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

        public string GetUserNameIdentifierClaim()
        {
            string? userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
    }
}
