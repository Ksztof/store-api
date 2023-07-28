using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public interface IProductsService
    {
        public Task<ProductDto> CreateProductAsync(CreateProductForm createProductForm);
        public Task<ProductDto> UpdateProductAsync(UpdateProductForm updateform);
        public Task DeleteProductAsync(int productId);
        public Task<ProductDto> GetProductByIdAsync(int productId);
        public Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}
