using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Shared.Errors;

namespace PerfumeStore.Infrastructure.Persistence.Repositories
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
            EntityEntry<Cart> cartEntry = _shopDbContext.Carts.Update(item);
            await _shopDbContext.SaveChangesAsync();

            return cartEntry.Entity;
        }
        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            Cart? cart = await _shopDbContext.Carts
                .AsSingleQuery()
                .Include(x => x.CartLines)
                .ThenInclude(x => x.Product)
                .SingleOrDefaultAsync(c => c.Id == cartId);

            return cart;
        }

        public async Task<Cart> GetByUserIdAsync(string userId)
        {
            Cart? cart = await _shopDbContext.Carts
                .AsSingleQuery()
                .Include(cl => cl.CartLines)
                .ThenInclude(p => p.Product)
                .SingleOrDefaultAsync(x => x.StoreUserId == userId);

            return cart;
        }

        public async Task<Cart> GetByUserEmailAsync(string email)
        {
            Cart? cart = await _shopDbContext.Carts
                .AsSingleQuery()
                .Include(cl => cl.CartLines)
                .ThenInclude(p => p.Product)
                .SingleOrDefaultAsync(x => x.StoreUser.Email == email);

            return cart;
        }

        public async Task DeleteAsync(Cart cart)
        {
            var result = _shopDbContext.Carts.Remove(cart);

            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<Result<DateTime>> GetCartDateByIdAsync(int cartId)
        {
            Cart? cart = await GetByIdAsync(cartId);
            if (cart == null)
            {
                Error error = EntityErrors<Cart, int>.NotFoundByCartId(cartId);
                return Result<DateTime>.Failure(error);
            }

            DateTime createdAt = cart.CreatedAt;

            return Result<DateTime>.Success(createdAt);
        }

        public async Task<int> GetCartIdByUserIdAsync(string userId)
        {
            int cartId = await _shopDbContext.Carts
                .Where(c => c.StoreUser.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            return cartId;
        }
    }
}