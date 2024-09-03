using Store.Domain.Abstractions;

namespace Store.Domain.Products
{
    public interface IProductsRepository
    {
        public Task<IEnumerable<Product>> GetByIdsAsync(int[] productIds);
        public Task<EntityResult<Product>> GetByName(string productName);
        public Task<Product> CreateAsync(Product product);

        public Task<EntityResult<Product>> GetByIdAsync(int productId);

        public Task<IEnumerable<Product>> GetAllAsync();

        public Task<Product> UpdateAsync(Product product);

        public Task DeleteAsync(int productId);
    }
}