namespace PerfumeStore.Core.Services
{
  public interface IGuestSessionService
  {
    public void SendCartIdToGuest(int cartId);

    public int? GetCartId();
  }
}