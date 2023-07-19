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

            return productEntry.Entity;
        }

        public async Task DeleteAsync(Product item)
        {
            EntityEntry<Product> deleteResult = _shopDbContext.Products.Remove(item);
            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = await _shopDbContext.Products.ToListAsync();
            return productsList;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product? product = await _shopDbContext.Products
                .AsSingleQuery()
                .Include(x => x.ProductCategories)
                .SingleOrDefaultAsync(x => x.Id == id);
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
