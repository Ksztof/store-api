using Store.Application.Carts.Dto.Request;
using Store.Application.Carts.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Carts.Dto.Response;

namespace Store.Application.Carts;

public interface ICartsService
{
    public Task<EntityResult<CartResponseDto>> AddProductsToCartAsync(NewProductsDtoApp request);
    public Task<EntityResult<CartResponseDto>> GetCartByIdAsync(int cartId);
    public Task<EntityResult<CartResponseDto>> DeleteProductFromCartAsync(int productId);
    public Task<EntityResult<CartResponseDto>> ModifyProductAsync(ModifyProductDtoApp productModification);
    public Task<EntityResult<AboutCartDomResDto>> CheckCartAsync();
    public Task<Result> ClearCartAsync();
    public Task<EntityResult<CartResponseDto>> AssignGuestCartToUserAsync(string userId, int cartId);
    public Task<EntityResult<AboutCartDomResDto>> ReplaceCartContentAsync(NewProductsDtoApp request);
    public Task<EntityResult<AboutCartDomResDto>> CheckCurrentCartAsync(CheckCurrentCartDtoApp addProductToCartDto);
}