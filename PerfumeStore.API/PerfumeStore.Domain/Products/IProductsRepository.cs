using PerfumeStore.Domain.Core;

namespace PerfumeStore.Domain.Products
{
    public interface IProductsRepository : IRepository<Product, int>
    {
    }
}