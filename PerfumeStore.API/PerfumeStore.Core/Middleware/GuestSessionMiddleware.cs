using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Middleware
{
    public class GuestSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public GuestSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (!httpContext.Request.Cookies.ContainsKey("GuestSessionId"))
            {
                var sessionId = Guid.NewGuid().ToString();
                httpContext.Response.Cookies.Append("GuestSessionId", sessionId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
            }

            await _next(httpContext);
        }
    }
}
