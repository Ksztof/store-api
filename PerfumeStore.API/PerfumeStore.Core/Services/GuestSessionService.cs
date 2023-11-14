using Microsoft.AspNetCore.Http;

namespace PerfumeStore.Core.Services
{
    public class GuestSessionService : IGuestSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GuestSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetCartId()
        {
            if (!_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("GuestSessionId"))
            {
                return null;
            }

            string stringCartId = _httpContextAccessor.HttpContext.Request.Cookies["GuestSessionId"];
            bool parseSuccess = int.TryParse(stringCartId, out int cartId);
            if (!parseSuccess)
            {
                throw new FormatException($"GuestSessionId is present but there were problems with parsing. Value: {stringCartId}");
            }

            return cartId;
        }

        public void SendCartIdToGuest(int cartId)
        {
            string stringCartId = cartId.ToString();
            _httpContextAccessor.HttpContext.Response.Cookies.Append("GuestSessionId", stringCartId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
        }
    }
}