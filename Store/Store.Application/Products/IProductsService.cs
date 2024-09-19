using Store.Application.Products.Dto.Request;
using Store.Application.Products.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Products;

namespace Store.Application.Products;

public interface IProductsService
{
    public Task<EntityResult<ProductResponseDto>> CreateProductAsync(CreateProductDtoApp createProductForm);
    public Task<EntityResult<Product>> DeleteProductAsync(int productId);
    public Task<EntityResult<ProductResponseDto>> GetProductByIdAsync(int productId);
    public Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    public Task<EntityResult<ProductResponseDto>> UpdateProductAsync(UpdateProductDtoApp updateForm);
}