using Store.Domain.Abstractions;

namespace Store.Domain.Shared.Errors
{
    public static class PaymentErrors
    {
        public static readonly Error MissingCartIdCookie = new("User.MissingCookieWithCartId", "Cookie with cart Id is missing");
    }
}
