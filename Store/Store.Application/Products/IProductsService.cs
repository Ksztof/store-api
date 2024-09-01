using Store.Application.Products.Dto.Request;
using Store.Application.Products.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Products;

namespace Store.Application.Products
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