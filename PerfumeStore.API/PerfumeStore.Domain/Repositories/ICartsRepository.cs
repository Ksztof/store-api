using PerfumeStore.Domain.Entities.Carts;

namespace PerfumeStore.Domain.Repositories
{
    public interface ICartsRepository
    {
        public Task<Cart> CreateAsync(Cart item);

        public Task<Cart> UpdateAsync(Cart item);

        public Task<Cart?> GetByIdAsync(int cartId);

        public Task<Cart> GetByUserIdAsync(string userEmail);

        public Task<Cart> GetByUserEmailAsync(string email);
        public Task DeleteAsync(Cart cart);
        public Task<DateTime> GetCartDateByIdAsync(int cartId);
    }
}