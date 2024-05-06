using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.Carts;

namespace PerfumeStore.Application.Carts
{
    public interface ICartsService
    {
        public Task<EntityResult<CartResponse>> AddProductsToCartAsync(NewProductsDtoApp request);

        public Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId);

        public Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId);

        public Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification);

        public Task<EntityResult<AboutCartDomRes>> CheckCartAsync();

        public Task<EntityResult<CartResponse>> ClearCartAsync();

        public Task<EntityResult<CartResponse>> AssignGuestCartToUserAsync(string userId, int cartId);
        public Task<EntityResult<AboutCartDomRes>> ReplaceCartContentAsync(NewProductsDtoApp request);
        public Task<EntityResult<AboutCartDomRes>> IsCurrentCartAsync(IsCurrentCartDtoApp addProductToCartDto);
    }
}