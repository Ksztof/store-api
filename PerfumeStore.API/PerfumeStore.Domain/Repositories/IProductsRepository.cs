using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Repositories.Generics;

namespace PerfumeStore.Domain.Repositories
{
    public interface IProductsRepository : IRepository<Product, int>
    {
        public Task<IEnumerable<Product>> GetByIdsAsync(int[] ids);
    }
}