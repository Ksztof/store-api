using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Application.Products
{
    public interface IProductsService
    {
        public Task<EntityResult<ProductResponse>> CreateProductAsync(CreateProductForm createProductForm);
        public Task<EntityResult<Product>> DeleteProductAsync(int productId);
        public Task<EntityResult<ProductResponse>> GetProductByIdAsync(int productId);
        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        public Task<EntityResult<ProductResponse>> UpdateProductAsync(UpdateProductForm updateForm, int productId);

    }
}