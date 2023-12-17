namespace PerfumeStore.Application.Cookies
{
    public interface IGuestSessionService
    {
        public void SendCartIdToGuest(int cartId);

        public int? GetCartId();
        public void SetCartIdCookieAsExpired();
    }
}