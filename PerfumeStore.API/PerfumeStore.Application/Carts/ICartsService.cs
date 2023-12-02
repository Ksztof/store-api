using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.Carts
{
    public interface ICartsService
    {
        public Task<EntityResult<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApplication request);
        public Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId);
        public Task<EntityResult<Cart>> GetCartByIdAsync(int cartId);

        public Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId);

        public Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApplication productModification);

        public Task<EntityResult<AboutCartRes>> CheckCartAsync();

        public Task<EntityResult<CartResponse>> ClearCartAsync();
    }
}