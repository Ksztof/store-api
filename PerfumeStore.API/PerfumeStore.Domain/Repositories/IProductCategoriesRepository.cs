using PerfumeStore.Domain.Entities.ProductCategories;

namespace PerfumeStore.Domain.Repositories
{
    public interface IProductCategoriesRepository
    {
        public Task<ProductCategory?> GetByIdAsync(int id);

        public Task<ICollection<ProductCategory>> GetByIdsAsync(ICollection<int> ids);
    }
}