using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.Products
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