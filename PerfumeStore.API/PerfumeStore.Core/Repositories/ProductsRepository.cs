using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        public ProductsRepository()
        {
        }

        public async Task<Product> CreateAsync(Product item)
        {
            item.ProductId = ProductIdGenerator.GetNextId() + 3;
            InMemoryDatabase.products.Add(item);

            return await Task.FromResult(item);
        }

        public async Task DeleteAsync(int id)
        {
            Product productToDelete = await GetByIdAsync(id);
            InMemoryDatabase.products.Remove(productToDelete);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = InMemoryDatabase.products;

            return await Task.FromResult(productsList);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product product = InMemoryDatabase.products.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return null;
            }
            return await Task.FromResult(product);
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            int productToUpdateIndex = InMemoryDatabase.products.IndexOf(item);
            InMemoryDatabase.products[productToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

        private int GetCurrentProductId()
        {
            int lastProductId = InMemoryDatabase.products.Max(x => x.ProductId);
            int currentProductId = lastProductId + 1;
            return currentProductId;
        }
    }
}
