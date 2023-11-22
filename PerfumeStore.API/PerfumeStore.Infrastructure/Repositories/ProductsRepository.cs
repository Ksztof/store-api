using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Infrastructure.Repositories
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

            return productEntry.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            Product product = await GetByIdAsync(id);
            EntityEntry<Product> deleteResult = _shopDbContext.Products.Remove(product);
            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = await _shopDbContext.Products
                .Select(x => x).ToListAsync();

            return productsList;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product? product = await _shopDbContext.Products
                .AsSingleQuery()
                .Include(x => x.ProductProductCategories)
                .SingleOrDefaultAsync(x => x.Id == id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(int[] ids)
        {
            IEnumerable<Product> product = await _shopDbContext.Products
                .AsSingleQuery()
                .Include(x => x.ProductProductCategories)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            return product;
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            EntityEntry<Product> productEntry = _shopDbContext.Products.Update(item);
            await _shopDbContext.SaveChangesAsync();

            return productEntry.Entity;
        }
    }
}