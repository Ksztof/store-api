using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        private readonly ShopDbContext _shopDbContext;

        public ProductCategoriesRepository(ShopDbContext shopDbContext)
        {
            _shopDbContext = shopDbContext;
        }

        public async Task<ProductCategory?> GetByIdAsync(int id)
        {
            ProductCategory? ProductCategory = await _shopDbContext.ProductCategories.SingleOrDefaultAsync(x => x.Id == id);
            return ProductCategory;
        }
    }
}
