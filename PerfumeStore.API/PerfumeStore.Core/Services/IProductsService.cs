using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public interface IProductsService
    {
        public Task<ProductResponse> CreateProductAsync(CreateProductForm createProductForm);

        public Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateform, int productId);

        public Task DeleteProductAsync(int productId);

        public Task<ProductResponse> GetProductByIdAsync(int productId);

        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    }
}