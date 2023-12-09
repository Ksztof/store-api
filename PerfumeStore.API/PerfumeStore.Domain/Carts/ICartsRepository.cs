using PerfumeStore.Domain.CarLines;

namespace PerfumeStore.Domain.Carts
{
    public interface ICartsRepository
    {
        public Task<Cart> CreateAsync(Cart item);

        public Task<Cart> UpdateAsync(Cart item);

        public Task<Cart?> GetByIdAsync(int cartId);

        public Task DeleteCartLineAsync(CartLine cartLine);

        public Task ClearCartAsync(ICollection<CartLine> cartLines);

        public Task<Cart> GetByUserIdAsync(string userEmail);
        public Task<Cart> GetByUserEmailAsync(string email);
    }
}