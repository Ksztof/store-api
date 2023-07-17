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

            /*if (cartEntry.State is not EntityState.Added)
            {
                throw new InvalidOperationException($"The entity is not in the Added state. Value cartEntry {cartEntry} ");
            }*/
            return cartEntry.Entity;
        }

        public async Task<Cart> UpdateAsync(Cart item)
        {
            EntityEntry<Cart?> cartEntry = _shopDbContext.Carts.Update(item);
            await _shopDbContext.SaveChangesAsync();

          /*  if (cartEntry.State is not EntityState.Modified)
            {
                throw new InvalidOperationException($"The Cart entity is not in the Modified state. Cart state {cartEntry.State} ");
            }*/
            return cartEntry.Entity;
        }

        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            Cart? cart = await _shopDbContext.Carts
                .Include(c => c.CartLines)
                .SingleOrDefaultAsync(c => c.Id == cartId);
            return cart;
        }
    }
}
