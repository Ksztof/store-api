using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Application.Products
{
    public interface IProductsService
    {
        public Task<EntityResult<ProductResponse>> CreateProductAsync(CreateProductDtoApp createProductForm);
        public Task<EntityResult<Product>> DeleteProductAsync(int productId);
        public Task<EntityResult<ProductResponse>> GetProductByIdAsync(int productId);
        public Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        public Task<EntityResult<ProductResponse>> UpdateProductAsync(UpdateProductDtoApp updateForm);

    }
}