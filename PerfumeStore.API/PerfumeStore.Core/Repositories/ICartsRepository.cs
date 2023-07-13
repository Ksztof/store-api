using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public interface ICartsRepository
    {
        public Task<Cart> CreateAsync(Cart item);
        public Task<Cart> UpdateAsync(Cart item);
        public Task<Cart?> GetByCartIdAsync(int cartId);
    }
}
