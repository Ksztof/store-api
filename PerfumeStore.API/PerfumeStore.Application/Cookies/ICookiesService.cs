namespace PerfumeStore.Application.Cookies
{
    public interface ICookiesService
    {
        public void SendCartIdToGuest(int cartId);

        public int? GetCartId();
    }
}