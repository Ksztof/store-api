using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            bool? userIsAuthenticated =  _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;
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
