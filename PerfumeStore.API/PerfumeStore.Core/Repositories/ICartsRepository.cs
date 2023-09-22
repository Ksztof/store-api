using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
  public interface ICartsRepository
  {
    public Task<Cart> CreateAsync(Cart item);

    public Task<Cart> UpdateAsync(Cart item);

    public Task<Cart?> GetByIdAsync(int cartId);

    public Task DeleteCartLineAsync(CartLine cartLine);

    public Task ClearCartAsync(ICollection<CartLine> cartLines);
  }
}