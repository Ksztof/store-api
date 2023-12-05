using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Domain.Errors
{
    public static class EmailErrors
    {
        public static readonly Error MissingCartIdCookie = new("Authentication.MissingCookieWithCartId", "Cookie with cart Id is missing");
    }
}
