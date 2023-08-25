using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public interface ICartsService
    {
        public Task<CartResponse> AddProductToCartAsync(int productId, decimal productQuantity);

        public Task<CartResponse> GetCartByIdAsync(int cartId);

        public Task<CartResponse> DeleteCartLineFromCartAsync(int productId);

        public Task<CartResponse> SetProductQuantityAsync(int productId, decimal productQuantity);

        public Task<AboutCartResponse> CheckCartAsync();

        public Task<CartResponse> ClearCartAsync();
    }
}