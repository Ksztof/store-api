using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Core.CustomExceptions;
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
                throw new InvalidOperationException($"The entity is not in the Added state. Value Product: {productEntry.Entity} ");
            }
            return productEntry.Entity;
        }

        public async Task DeleteAsync(Product item)
        {
            EntityEntry<Product> deleteResult = _shopDbContext.Products.Remove(item); 
            await _shopDbContext.SaveChangesAsync();
            if (deleteResult.State is not EntityState.Deleted)
            {
                throw new CantDeleteProductEx($"Can't delete Product. Product state: {deleteResult.State}");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = await _shopDbContext.Products.ToListAsync();
            return productsList;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product? product = await _shopDbContext.Products.SingleOrDefaultAsync(x => x.Id == id);
            return product;
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            EntityEntry<Product> productEntry = _shopDbContext.Products.Update(item);
            await _shopDbContext.SaveChangesAsync();

            if (productEntry.State is not EntityState.Modified)
            {
                throw new InvalidOperationException($"The Product entity is not in the Modified state. Product state: {productEntry.State} ");
            }
            return productEntry.Entity;
        }
    }
}
