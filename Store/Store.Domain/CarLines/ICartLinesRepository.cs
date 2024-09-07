namespace Store.Domain.CarLines;

public interface ICartLinesRepository
{
    public Task DeleteCartLineAsync(CartLine cartLine);
    public Task ClearCartAsync(ICollection<CartLine> cartLines);
    public Task<CartLine> GetByProductId(int productId);
}