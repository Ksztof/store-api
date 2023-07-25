namespace PerfumeStore.Domain.Models
{
    public interface IGuestSessionService
    {
        public void SendCartIdToGuest(int cartId);
        public int? GetCartId();
    }
}
