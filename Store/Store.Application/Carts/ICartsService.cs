using Store.Application.Carts.Dto.Request;
using Store.Application.Shared.DTO.Response;
using Store.Domain.Abstractions;
using Store.Domain.Shared.DTO.Response.Cart;

namespace Store.Application.Carts
{
    public interface ICartsService
    {
        public Task<EntityResult<CartResponse>> AddProductsToCartAsync(NewProductsDtoApp request);

        public Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId);

        public Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId);

        public Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification);

        public Task<EntityResult<AboutCartDomRes>> CheckCartAsync();

        public Task<Result> ClearCartAsync();

        public Task<EntityResult<CartResponse>> AssignGuestCartToUserAsync(string userId, int cartId);

        public Task<EntityResult<AboutCartDomRes>> ReplaceCartContentAsync(NewProductsDtoApp request);

        public Task<EntityResult<AboutCartDomRes>> IsCurrentCartAsync(CheckCurrentCartDtoApp addProductToCartDto);
    }
}