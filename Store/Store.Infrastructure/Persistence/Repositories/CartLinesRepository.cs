using Microsoft.EntityFrameworkCore;
using Store.Domain.CarLines;

namespace Store.Infrastructure.Persistence.Repositories;

public class CartLinesRepository : ICartLinesRepository
{
    private readonly ShopDbContext _shopDbContext;

    public CartLinesRepository(ShopDbContext shopDbContext)
    {
        _shopDbContext = shopDbContext;
    }

    public async Task DeleteCartLineAsync(CartLine cartLine)
    {
        _shopDbContext.CartsLine.Remove(cartLine);
        await _shopDbContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(ICollection<CartLine> cartLines)
    {
        _shopDbContext.CartsLine.RemoveRange(cartLines);
        await _shopDbContext.SaveChangesAsync();
    }

    public async Task<CartLine> GetByProductId(int productId)
    {
        CartLine? cartLine = await _shopDbContext.CartsLine
            .FirstOrDefaultAsync(cl => cl.ProductId == productId);

        return cartLine;
    }
}
