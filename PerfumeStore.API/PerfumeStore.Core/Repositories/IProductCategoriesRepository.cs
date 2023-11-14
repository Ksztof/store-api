using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
    public interface IProductCategoriesRepository
    {
        public Task<ProductCategory?> GetByIdAsync(int id);

        public Task<ICollection<ProductCategory>> GetByIdsAsync(ICollection<int> ids);
    }
}