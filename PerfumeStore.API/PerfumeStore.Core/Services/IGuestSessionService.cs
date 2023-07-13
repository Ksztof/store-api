namespace PerfumeStore.Core.Services
{
    public interface IGuestSessionService
    {
        public void SendCartId(int cartId);
        public int? GetCartId();
    }
}
