using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.Carts
{
    public interface ICartsService
    {
        public Task<CartResponse> AddProductToCartAsync(int productId, decimal productQuantity);

        public Task<CartResponse> GetCartResponseByIdAsync(int cartId);
        public Task<Cart> GetCartByIdAsync(int cartId);

        public Task<CartResponse> DeleteCartLineFromCartAsync(int productId);

        public Task<CartResponse> SetProductQuantityAsync(int productId, decimal productQuantity);

        public Task<AboutCartRes> CheckCartAsync();

        public Task<CartResponse> ClearCartAsync();
    }
}