using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Domain.Shared.Errors
{
    public static class PaymentErrors
    {
        public static readonly Error MissingCartIdCookie = new("User.MissingCookieWithCartId", "Cookie with cart Id is missing");
    }
}
