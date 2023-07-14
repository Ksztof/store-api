using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        public async Task<Cart?> CreateAsync(Cart item)
        {
            InMemoryDatabase.carts.Add(item);

            return await Task.FromResult(item);
        }

        public async Task<Cart> UpdateAsync(Cart item)
        {
            int cartToUpdateIndex = InMemoryDatabase.carts.IndexOf(item);
            InMemoryDatabase.carts[cartToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

        public async Task<Cart?> GetByCartIdAsync(int cartId)
        {
            Cart? cart = InMemoryDatabase.carts.FirstOrDefault(x => x.CartId == cartId);

            return await Task.FromResult(cart);
        }
    }
}
