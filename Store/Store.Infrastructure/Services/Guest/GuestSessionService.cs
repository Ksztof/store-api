using Microsoft.AspNetCore.Http;
using Store.Application.Contracts.Guest;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;

namespace Store.Infrastructure.Services.Guest
{
    public class GuestSessionService : IGuestSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GuestSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Result<int> GetCartId()
        {

            if (_httpContextAccessor.HttpContext == null)
            {
                return Result<int>.Failure(UserErrors.MissingHttpContext);
            }

            if (!_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("GuestSessionId"))
            {
                return Result<int>.Failure(UserErrors.MissingGuestSessionId);
            }

            string stringCartId = _httpContextAccessor.HttpContext.Request.Cookies["GuestSessionId"];
            bool parseSuccess = int.TryParse(stringCartId, out int cartId);

            if (!parseSuccess)
            {
                throw new FormatException($"GuestSessionId is present but there were problems with parsing. Value: {stringCartId}");
            }

            return Result<int>.Success(cartId);
        }

        public UserResult SendCartIdToGuest(int cartId)
        {
            string stringCartId = cartId.ToString();
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor?.HttpContext?.Response.Cookies
                    .Append("GuestSessionId", stringCartId,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMonths(1),
                        SameSite = SameSiteMode.None,
                        Secure = true,
                    });
                return UserResult.Success();
            }

            return UserResult.Failure(UserErrors.MissingHttpContext);
        }

        public UserResult SetCartIdCookieAsExpired()
        {
            if (_httpContextAccessor?.HttpContext?.Request.Cookies["GuestSessionId"] != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    "GuestSessionId",
                    "",
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(-1),
                        SameSite = SameSiteMode.None,
                        Secure = true,
                    }
                );

                return UserResult.Success();
            }

            return UserResult.Failure(UserErrors.MissingHttpContext);
        }
    }
}
