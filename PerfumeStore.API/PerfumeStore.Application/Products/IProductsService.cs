using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Application.Products
{
    public interface IProductsService
    {
        public Task<Result<ProductResponse>> CreateProductAsync(CreateProductForm createProductForm);
        public Task<Result<Product>> DeleteProductAsync(int productId);
        public Task<Result<ProductResponse>> GetProductByIdAsync(int productId);
        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        public Task<Result<ProductResponse>> UpdateProductAsync(UpdateProductForm updateForm, int productId);

    }
}