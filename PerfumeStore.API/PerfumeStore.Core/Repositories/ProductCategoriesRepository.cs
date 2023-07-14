using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        public Task<ProductCategories> CreateAsync(ProductCategories item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductCategories>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategories> GetById(int id)
        {
            ProductCategories? productCategory = InMemoryDatabase.productCategories.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(productCategory);
        }

        public async Task<ProductCategories> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategories> UpdateAsync(ProductCategories item)
        {
            throw new NotImplementedException();
        }
    }
}
