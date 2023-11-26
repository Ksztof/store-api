using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.Carts
{
    public interface ICartsService
    {
        public Task<Result<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApplication request);
        public Task<Result<CartResponse>> GetCartResponseByIdAsync(int cartId);
        public Task<Result<Cart>> GetCartByIdAsync(int cartId);

        public Task<Result<CartResponse>> DeleteCartLineFromCartAsync(int productId);

        public Task<Result<CartResponse>> ModifyProductAsync(ModifyProductDtoApplication productModification);

        public Task<Result<AboutCartRes>> CheckCartAsync();

        public Task<Result<CartResponse>> ClearCartAsync();
    }
}