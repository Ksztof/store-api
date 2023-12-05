using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Cookies
{
    public static class CookieErrors
    {
        public static readonly Error MissingCookie = new("Cookies.MissingCookie", "Can't find cookie");
    }
}
