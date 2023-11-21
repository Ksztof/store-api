using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.Products
{
    public interface IProductsService
    {
        public Task<Result> CreateProductAsync(CreateProductForm createProductForm);

        public Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateform, int productId);

        public Task<Result> DeleteProductAsync(int productId);

        public Task<ProductResponse> GetProductByIdAsync(int productId);

        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    }
}