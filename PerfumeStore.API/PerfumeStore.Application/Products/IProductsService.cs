using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Shared.Abstractions;

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