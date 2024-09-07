using Store.Application.Carts.Dto.Request;
using Store.Application.Carts.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Carts.Dto.Response;

namespace Store.Application.Carts;

public interface ICartsService
{
    public Task<EntityResult<CartResponse>> AddProductsToCartAsync(NewProductsDtoApp request);
    public Task<EntityResult<CartResponse>> GetCartByIdAsync(int cartId);
    public Task<EntityResult<CartResponse>> DeleteProductFromCartAsync(int productId);
    public Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification);
    public Task<EntityResult<AboutCartDomRes>> CheckCartAsync();
    public Task<Result> ClearCartAsync();
    public Task<EntityResult<CartResponse>> AssignGuestCartToUserAsync(string userId, int cartId);
    public Task<EntityResult<AboutCartDomRes>> ReplaceCartContentAsync(NewProductsDtoApp request);
    public Task<EntityResult<AboutCartDomRes>> CheckCurrentCartAsync(CheckCurrentCartDtoApp addProductToCartDto);
}