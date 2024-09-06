using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Store.Domain.Abstractions;
using Store.Domain.Carts;
using Store.Domain.Shared.Errors;

namespace Store.Infrastructure.Persistence.Repositories;

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

    public async Task<EntityResult<Cart>> GetByIdAsync(int cartId)
    {
        Cart? cart = await _shopDbContext.Carts
            .AsSingleQuery()
            .Include(x => x.CartLines)
            .ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(c => c.Id == cartId);

        if (cart is null)
        {
            Error error = EntityErrors<Cart, int>.NotFoundByCartId(cartId);

            return EntityResult<Cart>.Failure(error);
        }

        return EntityResult<Cart>.Success(cart);
    }

    public async Task<EntityResult<Cart>> GetByUserIdAsync(string userId)
    {
        Cart? cart = await _shopDbContext.Carts
            .AsSingleQuery()
            .Include(cl => cl.CartLines)
            .ThenInclude(p => p.Product)
            .SingleOrDefaultAsync(x => x.StoreUserId == userId);

        if (cart is null)
        {
            Error error = EntityErrors<Cart, int>.NotFoundByUserId(userId);

            return EntityResult<Cart>.Failure(error);
        }

        return EntityResult<Cart>.Success(cart);
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
        EntityResult<Cart> getCart = await GetByIdAsync(cartId);

        if (getCart.IsFailure)
        {
            return Result<DateTime>.Failure(getCart.Error);
        }

        DateTime createdAt = getCart.Entity.CreatedAt;

        return Result<DateTime>.Success(createdAt);
    }

    public async Task<Result<int>> GetCartIdByUserIdAsync(string userId)
    {
        int cartId = await _shopDbContext.Carts
            .Where(c => c.StoreUser.Id == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (cartId == 0)
        {
            Error error = EntityErrors<Cart, int>.EntityIdNotFoundByUserId(userId);
            return Result<int>.Failure(error);
        }

        return Result<int>.Success(cartId);
    }
}