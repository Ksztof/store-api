using Microsoft.AspNetCore.Http;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public interface ICookieService
    {
        public Result SetCookieWithToken(string token);
        public Result SetExpiredAuthToken(CookieOptions options);
    }
}
