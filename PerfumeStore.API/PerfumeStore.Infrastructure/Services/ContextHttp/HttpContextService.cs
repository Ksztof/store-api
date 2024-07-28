using Microsoft.AspNetCore.Http;
using PerfumeStore.Application.Contracts.ContextHttp;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
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

        public Result IsUserAuthenticated()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Result<string>.Failure(UserErrors.MissingHttpContext);
            }

            bool userIsAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            if (!userIsAuthenticated)
            {
                return Result.Failure(UserErrors.UserNotAuthenticated);
            }

            return Result.Success();
        }

        public Result<string> GetUserId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Result<string>.Failure(UserErrors.MissingHttpContext);
            }

            string userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Result<string>.Failure(UserErrors.MissingNameIdentifier);
            }

            return Result<string>.Success(userId);
        }

        public Result<string> GetActualProtocol()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Result<string>.Failure(UserErrors.MissingHttpContext);
            }

            string? protocol = _httpContextAccessor?.HttpContext?.Request.Scheme;
            if(protocol == null)
            {
                return Result<string>.Failure(UserErrors.MissingHttpProtocol);
            }

            return Result<string>.Success(protocol);
        }

        public Result SendCookieWithToken(string token, CookieOptions cookieOptions)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Result.Failure(UserErrors.MissingHttpContext);
            }

            string cookieName = "AuthCookie";

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(cookieName, token, cookieOptions);

            return Result.Success();    
        }
    }
}
