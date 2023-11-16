using PerfumeStore.Core.GenericInterfaces;

namespace PerfumeStore.Domain.Products
{
    public interface IProductsRepository : IRepository<Product, int>
    {
    }
}