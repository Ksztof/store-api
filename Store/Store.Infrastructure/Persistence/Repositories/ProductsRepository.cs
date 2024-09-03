using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Store.Domain.Abstractions;
using Store.Domain.Products;
using Store.Domain.Shared.Errors;

namespace Store.Infrastructure.Persistence.Repositories
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

        public async Task DeleteAsync(int productId)
        {
            EntityResult<Product> getProduct = await GetByIdAsync(productId);
            if (getProduct.IsFailure)
            {
                throw new KeyNotFoundException($"Product Not found by id, product id: {productId}");
            }

            EntityEntry<Product> deleteResult = _shopDbContext.Products.Remove(getProduct.Entity);
            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = await _shopDbContext.Products.ToListAsync();

            return productsList;
        }

        public async Task<EntityResult<Product>> GetByIdAsync(int productId)
        {
            Product? product = await _shopDbContext.Products
                .AsSingleQuery()
                .Include(x => x.ProductProductCategories)
                .SingleOrDefaultAsync(x => x.Id == productId);

            if(product is null)
            {
                Error error = EntityErrors<Product, int>.NotFoundByProductId(productId);
                return EntityResult<Product>.Failure(error);
            }

            return EntityResult<Product>.Success(product);
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

        public async Task<EntityResult<Product>> GetByName(string productName)
        {
            Product? product = await _shopDbContext.Products
                .FirstOrDefaultAsync(x => x.Name == productName);

            if(product is null) 
            {
                Error error = EntityErrors<Product, int>.NotFoundByName(productName);
                return EntityResult<Product>.Failure(error);
            }

            return EntityResult<Product>.Success(product);
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            EntityEntry<Product> productEntry = _shopDbContext.Products.Update(item);
            await _shopDbContext.SaveChangesAsync();

            return productEntry.Entity;
        }
    }
}