using Microsoft.AspNetCore.Http;

namespace PerfumeStore.Application.Contracts.HttpContext
{
    public interface IHttpContextService
    {
        public bool IsUserAuthenticated();

        public string GetUserId();

        public string GetActualProtocol();

        public void SendCookieWithToken(string token, CookieOptions cookieOptions);
    }
}