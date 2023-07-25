using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public interface IProductsService
    {
        public Task<Product> CreateProductAsync(CreateProductForm createProductForm);
        public Task<Product> UpdateProductAsync(UpdateProductForm updateform);
        public Task DeleteProductAsync(int productId);
        public Task<Product> GetProductByIdAsync(int productId);
        public Task<IEnumerable<Product>> GetAllProductsAsync();
    }
}
