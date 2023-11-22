using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Application.Products
{
    public interface IProductsService
    {
        public Task<Result<Product>> CreateProductAsync(CreateProductForm createProductForm);

        public Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateform, int productId);

        public Task<Result<Product>> DeleteProductAsync(int productId);

        public Task<ProductResponse> GetProductByIdAsync(int productId);

        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    }
}