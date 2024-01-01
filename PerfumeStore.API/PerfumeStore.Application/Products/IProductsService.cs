using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Entities.Products;

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