using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ShopDbContext _shopDbContext;
        public ProductsRepository(ShopDbContext shopDbContext)
        {
            _shopDbContext = shopDbContext;
        }

        public async Task<Product> CreateAsync(Product item)
        {
            EntityEntry<Product> productEntry = await _shopDbContext.AddAsync(item);
            await _shopDbContext.SaveChangesAsync();
            if (productEntry.State is not EntityState.Added)
            {
                throw new InvalidOperationException($"The entity is not in the Added state. Value productEntry: {productEntry} ");
            }
            return await Task.FromResult(item);
        }

        public async Task DeleteAsync(int id)
        {
            Product productToDelete = await GetByIdAsync(id);
            ShopDbContext.products.Remove(productToDelete);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = ShopDbContext.products;

            return await Task.FromResult(productsList);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product product = ShopDbContext.products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return null;
            }
            return await Task.FromResult(product);
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            int productToUpdateIndex = ShopDbContext.products.IndexOf(item);
            ShopDbContext.products[productToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

        private int GetCurrentProductId()
        {
            int lastProductId = ShopDbContext.products.Max(x => x.Id);
            int currentProductId = lastProductId + 1;
            return currentProductId;
        }
    }
}
