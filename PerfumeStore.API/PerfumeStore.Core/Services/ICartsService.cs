using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public interface ICartsService
    {
        public Task<CartDto> AddProductToCartAsync(int productId, decimal productQuantity);
        public Task<Cart> GetCartByIdAsync(int cartId);
        public Task<CartDto> DeleteCartLineFromCartAsync(int productId);
        public Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity);
        public Task<CheckCartForm> CheckCartAsync();
        public Task<Cart> ClearCartAsync();
    }
}
