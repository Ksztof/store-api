namespace PerfumeStore.Application.Cookies
{
    public interface ICookieService
    {
        public void SendCartIdToGuest(int cartId);

        public int? GetCartId();
    }
}