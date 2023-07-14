using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        public async Task<Cart?> CreateAsync(Cart item)
        {
            ShopDbContext.carts.Add(item);

            return await Task.FromResult(item);
        }

        public async Task<Cart> UpdateAsync(Cart item)
        {
            int cartToUpdateIndex = ShopDbContext.carts.IndexOf(item);
            ShopDbContext.carts[cartToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

        public async Task<Cart?> GetByCartIdAsync(int cartId)
        {

            Cart? cart = ShopDbContext.carts.FirstOrDefault(x => x.Id == cartId);

            return await Task.FromResult(cart);
        }
    }
}
