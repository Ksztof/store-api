using PerfumeStore.Domain.Entities.CarLines;

namespace PerfumeStore.Domain.Repositories
{
    public interface ICartLinesRepository
    {
        public Task DeleteCartLineAsync(CartLine cartLine);
        public Task ClearCartAsync(ICollection<CartLine> cartLines);
        public Task<CartLine> GetByProductId(int productId);

    }
}