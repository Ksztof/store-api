using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Shared.Abstractions;

namespace PerfumeStore.Application.Carts
{
    public interface ICartsService
    {
        public Task<EntityResult<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApp request);

        public Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId);

        public Task<EntityResult<Cart>> GetCartByIdAsync(int cartId);

        public Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId);

        public Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification);

        public Task<EntityResult<AboutCartDomRes>> CheckCartAsync();

        public Task<EntityResult<CartResponse>> ClearCartAsync();

        public Task<EntityResult<CartResponse>> AssignCartToUserAsync(string userId, int cartId);
    }
}