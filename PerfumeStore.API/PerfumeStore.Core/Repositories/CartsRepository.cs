using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        private readonly ShopDbContext _shopDbContext;

        public CartsRepository(ShopDbContext shopDbContext)
        {
            _shopDbContext = shopDbContext;
        }

        public async Task<Cart?> CreateAsync(Cart item)
        {
            EntityEntry<Cart?> cartEntry = await _shopDbContext.Carts.AddAsync(item);
            await _shopDbContext.SaveChangesAsync();

            return cartEntry.Entity;
        }

        public async Task<Cart> UpdateAsync(Cart item)
        {
            EntityEntry<Cart?> cartEntry = _shopDbContext.Carts.Update(item);
            await _shopDbContext.SaveChangesAsync();

            return cartEntry.Entity;
        }

        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            Cart? cart = await _shopDbContext.Carts
                .AsSingleQuery()
                .Include(c => c.CartLines).ThenInclude(p => p.Product)
                .SingleOrDefaultAsync(c => c.Id == cartId);
            return cart;
        }
        public async Task DeleteCartLineAsync(CartLine cartLine)
        {
            _shopDbContext.CartsLine.Remove(cartLine);
            await _shopDbContext.SaveChangesAsync();
        }
    }
}
