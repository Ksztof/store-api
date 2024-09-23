using AutoMapper;
using Store.Application.Carts.Dto.Request;
using Store.Application.Carts.Dto.Response;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Guest;
using Store.Domain.Abstractions;
using Store.Domain.CarLines;
using Store.Domain.Carts;
using Store.Domain.Carts.Dto.Response;
using Store.Domain.Products;
using Store.Domain.Products.Dto.Request;
using Store.Domain.Shared.Errors;
using Store.Domain.StoreUsers.Errors;

namespace Store.Application.Carts;

public class CartsService : ICartsService
{
    private readonly ICartsRepository _cartsRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IGuestSessionService _guestSessionService;
    private readonly IMapper _mapper;
    private readonly IHttpContextService _contextService;
    private readonly ICartLinesRepository _cartLinesRepository;

    public CartsService(
        ICartsRepository cartsRepository,
        IProductsRepository productsRepository,
        IGuestSessionService guestSessionService,
        IMapper mapper,
        IHttpContextService httpContextService,
        ICartLinesRepository cartLinesRepository)
    {
        _cartsRepository = cartsRepository;
        _productsRepository = productsRepository;
        _guestSessionService = guestSessionService;
        _mapper = mapper;
        _contextService = httpContextService;
        _cartLinesRepository = cartLinesRepository;
    }

    public async Task<EntityResult<CartResponseDto>> AddProductsToCartAsync(NewProductsDtoApp request)
    {
        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();
        IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);
        int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

        if (newProductsIds.Count() != existingProductsIds.Count())
        {
            int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

            return EntityResult<CartResponseDto>.Failure(EntityErrors<Product, int>.NotFoundEntitiesByIds(missingProdIds));
        }

        NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getUserIdResult.Error);
            }

            string? userId = getUserIdResult.Value;

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

            Cart userCart;

            if (getUserCart.IsFailure)
            {
                userCart = new Cart { StoreUserId = userId };

                userCart.AddProducts(newProductsIds);
                userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

                userCart = await _cartsRepository.CreateAsync(userCart);
            }
            else
            {
                userCart = getUserCart.Entity;

                userCart.AddProducts(newProductsIds);
                userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

                userCart = await _cartsRepository.UpdateAsync(userCart);
            }

            CartResponseDto userCartContents = MapCartResponse(userCart);

            return EntityResult<CartResponseDto>.Success(userCartContents);
        }

        Cart? guestCart;

        if (receiveGuestCartIdResult.IsSuccess)
        {
            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getGuestCart.Error);
            }

            guestCart = getGuestCart.Entity;

            guestCart.AddProducts(newProductsIds);
            guestCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

            guestCart = await _cartsRepository.UpdateAsync(guestCart);
        }
        else
        {
            guestCart = new Cart();

            guestCart.AddProducts(newProductsIds);
            guestCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

            guestCart = await _cartsRepository.CreateAsync(guestCart);

            _guestSessionService.SendCartIdToGuest(guestCart.Id);
        }

        CartResponseDto guestCartContents = MapCartResponse(guestCart);

        return EntityResult<CartResponseDto>.Success(guestCartContents);
    }

    public async Task<EntityResult<AboutCartDomResDto>> ReplaceCartContentAsync(NewProductsDtoApp request)
    {
        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();

        IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);

        int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

        if (newProductsIds.Count() != existingProductsIds.Count())
        {
            int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

            return EntityResult<AboutCartDomResDto>.Failure(EntityErrors<Product, int>.NotFoundEntitiesByIds(missingProdIds));
        }

        NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<AboutCartDomResDto>.Failure(getUserIdResult.Error);
            }

            string userId = getUserIdResult.Value;

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

            Cart userCart;

            if (getUserCart.IsFailure)
            {
                userCart = new Cart { StoreUserId = userId };

                userCart.AddProducts(newProductsIds);
                userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

                userCart = await _cartsRepository.CreateAsync(userCart);
            }
            else
            {
                userCart = getUserCart.Entity;

                userCart.ReplaceProducts(newProductsIds);
                userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

                userCart = await _cartsRepository.UpdateAsync(userCart);
            }

            AboutCartDomResDto userCartDetails = userCart.CheckCart();

            return EntityResult<AboutCartDomResDto>.Success(userCartDetails);
        }

        Cart? guestCart;

        if (receiveGuestCartIdResult.IsSuccess)
        {
            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<AboutCartDomResDto>.Failure(getGuestCart.Error);
            }

            guestCart = getGuestCart.Entity;

            guestCart.ReplaceProducts(newProductsIds);
            guestCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

            guestCart = await _cartsRepository.UpdateAsync(guestCart);
        }
        else
        {
            guestCart = new Cart();

            guestCart.AddProducts(newProductsIds);
            guestCart.UpdateProductsQuantity(addProductsToCartDtoDomain);

            guestCart = await _cartsRepository.CreateAsync(guestCart);

            _guestSessionService.SendCartIdToGuest(guestCart.Id);
        }

        AboutCartDomResDto guestCartDetails = guestCart.CheckCart();

        return EntityResult<AboutCartDomResDto>.Success(guestCartDetails);
    }

    public async Task<EntityResult<CartResponseDto>> DeleteProductFromCartAsync(int productId)
    {
        if (productId <= 0)
        {
            return EntityResult<CartResponseDto>.Failure(EntityErrors<Product, int>.WrongEntityId(productId));
        }

        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
        {
            Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

            return EntityResult<CartResponseDto>.Failure(error);
        }

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getUserIdResult.Error);
            }

            string userId = getUserIdResult.Value;

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

            if (getUserCart.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getUserCart.Error);
            }

            Cart userCart = getUserCart.Entity;

            CartLine? userCartLine = userCart?.CartLines?.FirstOrDefault(cl => cl.ProductId == productId);

            if (userCartLine == null)
            {
                return EntityResult<CartResponseDto>.Failure(EntityErrors<CartLine, int>.NotFound(productId));
            }

            userCart.DeleteCartLineFromCart(productId);

            await _cartLinesRepository.DeleteCartLineAsync(userCartLine);
            await _cartsRepository.UpdateAsync(userCart);

            CartResponseDto userCartContents = MapCartResponse(userCart);

            return EntityResult<CartResponseDto>.Success(userCartContents);
        }

        EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

        if (getGuestCart.IsFailure)
        {
            return EntityResult<CartResponseDto>.Failure(getGuestCart.Error);
        }

        Cart guestCart = getGuestCart.Entity;

        CartLine? cartLine = guestCart.CartLines.FirstOrDefault(x => x.ProductId == productId);

        if (cartLine == null)
        {
            return EntityResult<CartResponseDto>.Failure(EntityErrors<CartLine, int>.NotFoundByProductId(productId));
        }

        guestCart.DeleteCartLineFromCart(productId);
        await _cartLinesRepository.DeleteCartLineAsync(cartLine);
        guestCart = await _cartsRepository.UpdateAsync(guestCart);

        CartResponseDto guestCartContents = MapCartResponse(guestCart);

        return EntityResult<CartResponseDto>.Success(guestCartContents);
    }

    public async Task<EntityResult<CartResponseDto>> GetCartByIdAsync(int cartId)
    {
        if (cartId <= 0)
        {
            return EntityResult<CartResponseDto>.Failure(EntityErrors<Cart, int>.WrongEntityId(cartId));
        }

        EntityResult<Cart> getCart = await _cartsRepository.GetByIdAsync(cartId);

        if (getCart.IsFailure)
        {
            return EntityResult<CartResponseDto>.Failure(getCart.Error);
        }

        CartResponseDto cartResponse = MapCartResponse(getCart.Entity);

        return EntityResult<CartResponseDto>.Success(cartResponse);
    }

    public async Task<EntityResult<CartResponseDto>> ModifyProductAsync(ModifyProductDtoApp productModification)
    {
        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
        {
            Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

            return EntityResult<CartResponseDto>.Failure(error);
        }

        ModifyProductDtoDom modifiedProductForDomain = _mapper.Map<ModifyProductDtoDom>(productModification);

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getUserIdResult.Error);
            }

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(getUserIdResult.Value);

            if (getUserCart.IsFailure)
            {
                return EntityResult<CartResponseDto>.Failure(getUserCart.Error);
            }

            Cart userCart = getUserCart.Entity;

            userCart.ModifyProduct(modifiedProductForDomain);
            userCart = await _cartsRepository.UpdateAsync(userCart);

            CartResponseDto userCartContents = MapCartResponse(userCart);

            return EntityResult<CartResponseDto>.Success(userCartContents);
        }

        EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

        if (getGuestCart.IsFailure)
        {
            return EntityResult<CartResponseDto>.Failure(getGuestCart.Error);
        }

        Cart guestCart = getGuestCart.Entity;

        guestCart.ModifyProduct(modifiedProductForDomain);
        guestCart = await _cartsRepository.UpdateAsync(guestCart);

        CartResponseDto guestCartContents = MapCartResponse(guestCart);

        return EntityResult<CartResponseDto>.Success(guestCartContents);
    }

    public async Task<EntityResult<AboutCartDomResDto>> CheckCartAsync()
    {
        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
        {
            Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

            return EntityResult<AboutCartDomResDto>.Failure(error);
        }

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<AboutCartDomResDto>.Failure(getUserIdResult.Error);
            }

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(getUserIdResult.Value);

            if (getUserCart.IsFailure || getUserCart.Entity.CartLines is null || !getUserCart.Entity.CartLines.Any())
            {
                return EntityResult<AboutCartDomResDto>.Success();
            }

            AboutCartDomResDto userCartDetails = getUserCart.Entity.CheckCart();

            return EntityResult<AboutCartDomResDto>.Success(userCartDetails);
        }

        EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

        if (getGuestCart.IsFailure)
        {
            return EntityResult<AboutCartDomResDto>.Failure(getGuestCart.Error);
        }

        AboutCartDomResDto guestCartDetails = getGuestCart.Entity.CheckCart();

        return EntityResult<AboutCartDomResDto>.Success(guestCartDetails);
    }

    public async Task<Result> ClearCartAsync()
    {
        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
        {
            Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

            return Result.Failure(error);
        }

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return Result.Failure(getUserIdResult.Error);
            }

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(getUserIdResult.Value);

            if (getUserCart.IsFailure)
            {
                //if cart can't be fount it means that cart content has been already cleared
                return Result.Success();
            }

            Cart userCart = getUserCart.Entity;

            ICollection<CartLine> userCartLines = userCart.CartLines;

            if (userCartLines.Any())
            {
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);
            }

            CartResponseDto userCartContents = MapCartResponse(userCart);

            return Result.Success();
        }

        EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

        if (getGuestCart.IsFailure)
        {
            return Result.Failure(getGuestCart.Error);
        }

        Cart guestCart = getGuestCart.Entity;

        ICollection<CartLine> guestCartLines = guestCart.CartLines;

        if (guestCartLines.Any())
        {
            guestCart.ClearCart();
            await _cartsRepository.UpdateAsync(guestCart);
        }

        CartResponseDto guestCartContents = MapCartResponse(guestCart);

        return Result.Success();
    }

    public async Task<EntityResult<CartResponseDto>> AssignGuestCartToUserAsync(string userId, int cartId)
    {
        EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(cartId);

        if (getGuestCart.IsFailure)
        {
            return EntityResult<CartResponseDto>.Failure(getGuestCart.Error);
        }

        Cart guestCart = getGuestCart.Entity;

        EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

        if (getUserCart.IsFailure)
        {
            guestCart.AssignUserToCart(userId);
            await _cartsRepository.UpdateAsync(guestCart);

            return EntityResult<CartResponseDto>.Success();
        }

        Cart userCart = getUserCart.Entity;

        int[] newProductsIds = guestCart.CartLines.Select(cl => cl.ProductId).ToArray();
        NewProductsDtoDom productsAndQuantities = GetProductsAndQuantitiesToAssign(guestCart);

        userCart.AddProducts(newProductsIds);
        userCart.UpdateProductsQuantity(productsAndQuantities);
        await _cartsRepository.UpdateAsync(userCart);

        return EntityResult<CartResponseDto>.Success();
    }

    public async Task<EntityResult<AboutCartDomResDto>> CheckCurrentCartAsync(CheckCurrentCartDtoApp addProductToCartDto)
    {
        int cartId = 0;

        Result isUserAuthenticated = _contextService.IsUserAuthenticated();
        Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

        if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
        {
            Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

            return EntityResult<AboutCartDomResDto>.Failure(error);
        }

        if (isUserAuthenticated.IsSuccess)
        {
            Result<string> getUserIdResult = _contextService.GetUserId();

            if (getUserIdResult.IsFailure)
            {
                return EntityResult<AboutCartDomResDto>.Failure(getUserIdResult.Error);
            }


            Result<int> getCartId = await _cartsRepository.GetCartIdByUserIdAsync(getUserIdResult.Value);

            if (getCartId.IsFailure)
            {
                return EntityResult<AboutCartDomResDto>.Failure(getCartId.Error);
            }

            cartId = getCartId.Value;
        }

        if (receiveGuestCartIdResult.IsSuccess)
        {
            cartId = receiveGuestCartIdResult.Value;
        }

        Result<DateTime> createdAtDb = await _cartsRepository.GetCartDateByIdAsync(cartId);

        if (createdAtDb == null)
        {
            Error error = EntityErrors<Cart, int>.NotFoundByCartId(cartId);

            return EntityResult<AboutCartDomResDto>.Failure(error);
        }

        bool isCartActual = IsCreationDateActual(addProductToCartDto.CreatedAt, createdAtDb.Value);

        if (isCartActual)
        {
            return EntityResult<AboutCartDomResDto>.Success();
        }

        EntityResult<Cart> getCart = await _cartsRepository.GetByIdAsync(cartId);

        if (getCart.IsFailure)
        {
            return EntityResult<AboutCartDomResDto>.Success();
        }

        AboutCartDomResDto userCartDetails = getCart.Entity.CheckCart();

        return EntityResult<AboutCartDomResDto>.Success(userCartDetails);
    }

    private static NewProductsDtoDom GetProductsAndQuantitiesToAssign(Cart? guestCart)
    {
        return new NewProductsDtoDom
        {
            Products = guestCart.CartLines.Select(cl => new ProductInCartDom
            {
                ProductId = cl.ProductId,
                Quantity = cl.Quantity,
            }).ToArray()
        };
    }

    private CartResponseDto MapCartResponse(Cart cart)
    {
        return new CartResponseDto
        {
            CartId = cart.Id,
            CartLineResponse = _mapper.Map<IEnumerable<CartLineResponseDto>>(cart.CartLines),
        };
    }

    private static bool IsCreationDateActual(DateTime requestCartDate, DateTime dbCartDate)
    {
        return requestCartDate == dbCartDate;
    }
}