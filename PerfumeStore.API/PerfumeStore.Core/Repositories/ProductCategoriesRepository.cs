using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        public Task<ProductCategory> CreateAsync(ProductCategory item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory> GetById(int id)
        {
            ProductCategory? productCategory = ShopDbContext.productCategories.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(productCategory);
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory> UpdateAsync(ProductCategory item)
        {
            throw new NotImplementedException();
        }
    }
}
