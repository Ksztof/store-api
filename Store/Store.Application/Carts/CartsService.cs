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

namespace Store.Application.Carts
{
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

        public async Task<EntityResult<CartResponse>> AddProductsToCartAsync(NewProductsDtoApp request)
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();
            IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);
            int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != existingProductsIds.Count())
            {
                int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

                return EntityResult<CartResponse>.Failure(EntityErrors<Product, int>.NotFoundEntitiesByIds(missingProdIds));
            }

            NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(result.Error);
                }

                string? userId = result.Value;

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

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart;

            if (receiveGuestCartIdResult.IsSuccess)
            {
                EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

                if (getGuestCart.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(getGuestCart.Error);
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

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<AboutCartDomRes>> ReplaceCartContentAsync(NewProductsDtoApp request)
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();
            IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);
            int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != existingProductsIds.Count())
            {
                int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

                return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Product, int>.NotFoundEntitiesByIds(missingProdIds));
            }

            NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<AboutCartDomRes>.Failure(result.Error);
                }

                string userId = result.Value;
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

                AboutCartDomRes userCartDetails = userCart.CheckCart();

                return EntityResult<AboutCartDomRes>.Success(userCartDetails);
            }

            Cart? guestCart;

            if (receiveGuestCartIdResult.IsSuccess)
            {
                EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

                if (getGuestCart.IsFailure)
                {
                    return EntityResult<AboutCartDomRes>.Failure(getGuestCart.Error);
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

            AboutCartDomRes guestCartDetails = guestCart.CheckCart();

            return EntityResult<AboutCartDomRes>.Success(guestCartDetails);
        }

        public async Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId)
        {
            if (productId <= 0)
                return EntityResult<CartResponse>.Failure(EntityErrors<Product, int>.WrongEntityId(productId));

            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(result.Error);
                }

                string userId = result.Value;

                EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (getUserCart.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(getUserCart.Error);
                }

                Cart userCart = getUserCart.Entity;

                CartLine? userCartLine = userCart?.CartLines?.FirstOrDefault(cl => cl.ProductId == productId);

                if (userCartLine == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.NotFound(productId));
                }

                userCart.DeleteCartLineFromCart(productId);
                await _cartLinesRepository.DeleteCartLineAsync(userCartLine);
                await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<CartResponse>.Failure(getGuestCart.Error);
            }

            Cart guestCart = getGuestCart.Entity;

            CartLine? cartLine = guestCart.CartLines.FirstOrDefault(x => x.ProductId == productId);

            if (cartLine == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.NotFoundByProductId(productId));
            }

            guestCart.DeleteCartLineFromCart(productId);
            await _cartLinesRepository.DeleteCartLineAsync(cartLine);
            guestCart = await _cartsRepository.UpdateAsync(guestCart);

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId)
        {
            if (cartId <= 0)
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.WrongEntityId(cartId));

            EntityResult<Cart> getCart = await _cartsRepository.GetByIdAsync(cartId);
            if (getCart.IsFailure)
            {
                return EntityResult<CartResponse>.Failure(getCart.Error);
            }

            CartResponse cartResponse = MapCartResponse(getCart.Entity);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification)
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<CartResponse>.Failure(error);
            }

            ModifyProductDtoDom modifiedProductForDomain = _mapper.Map<ModifyProductDtoDom>(productModification);

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(result.Error);
                }

                EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(result.Value);

                if (getUserCart.IsFailure)
                {
                    return EntityResult<CartResponse>.Failure(getUserCart.Error);
                }

                Cart userCart = getUserCart.Entity;

                userCart.ModifyProduct(modifiedProductForDomain);
                userCart = await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<CartResponse>.Failure(getGuestCart.Error);
            }

            Cart guestCart = getGuestCart.Entity;

            guestCart.ModifyProduct(modifiedProductForDomain);
            guestCart = await _cartsRepository.UpdateAsync(guestCart);

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<AboutCartDomRes>> CheckCartAsync()
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<AboutCartDomRes>.Failure(error);
            }

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<AboutCartDomRes>.Failure(result.Error);
                }

                EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(result.Value);

                if (getUserCart.IsFailure || getUserCart.Entity.CartLines is null || !getUserCart.Entity.CartLines.Any())
                {
                    return EntityResult<AboutCartDomRes>.Success();
                }

                AboutCartDomRes userCartDetails = getUserCart.Entity.CheckCart();

                return EntityResult<AboutCartDomRes>.Success(userCartDetails);
            }

            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<AboutCartDomRes>.Failure(getGuestCart.Error);
            }

            AboutCartDomRes guestCartDetails = getGuestCart.Entity.CheckCart();

            return EntityResult<AboutCartDomRes>.Success(guestCartDetails);
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
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }

                EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(result.Value);
                if (getUserCart.IsFailure)
                {
                    return Result.Failure(getUserCart.Error);
                }

                Cart userCart = getUserCart.Entity;

                ICollection<CartLine> userCartLines = userCart.CartLines;

                if (userCartLines.Any())
                {
                    userCart.ClearCart();
                    await _cartsRepository.UpdateAsync(userCart);
                }

                CartResponse userCartContents = MapCartResponse(userCart);

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

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return Result.Success();
        }

        public async Task<EntityResult<CartResponse>> AssignGuestCartToUserAsync(string userId, int cartId)
        {
            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(cartId);

            if (getGuestCart.IsFailure)
            {
                return EntityResult<CartResponse>.Failure(getGuestCart.Error);
            }

            Cart guestCart = getGuestCart.Entity;

            EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);

            if (getUserCart.IsFailure)
            {
                guestCart.AssignUserToCart(userId);
                await _cartsRepository.UpdateAsync(guestCart);

                return EntityResult<CartResponse>.Success();
            }

            Cart userCart = getUserCart.Entity;

            int[] newProductsIds = guestCart.CartLines.Select(cl => cl.ProductId).ToArray();
            NewProductsDtoDom productsAndQuantities = GetProductsAndQuantitiesToAssign(guestCart);

            userCart.AddProducts(newProductsIds);
            userCart.UpdateProductsQuantity(productsAndQuantities);
            await _cartsRepository.UpdateAsync(userCart);

            return EntityResult<CartResponse>.Success();
        }

        public async Task<EntityResult<AboutCartDomRes>> IsCurrentCartAsync(CheckCurrentCartDtoApp addProductToCartDto)
        {
            int cartId = 0;
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();

            Result<int> receiveGuestCartIdResult = _guestSessionService.GetCartId();

            if (receiveGuestCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<AboutCartDomRes>.Failure(error);
            }

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<AboutCartDomRes>.Failure(result.Error);
                }


                Result<int> getCartId = await _cartsRepository.GetCartIdByUserIdAsync(result.Value);

                if (getCartId.IsFailure)
                {
                    return EntityResult<AboutCartDomRes>.Failure(getCartId.Error);
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

                return EntityResult<AboutCartDomRes>.Failure(error);
            }

            bool isCartActual = IsCreationDateActual(addProductToCartDto.CreatedAt, createdAtDb.Value);

            if (isCartActual)
            {
                return EntityResult<AboutCartDomRes>.Success();
            }

            EntityResult<Cart> getCart = await _cartsRepository.GetByIdAsync(cartId);
            if (getCart.IsFailure)
            {
                return EntityResult<AboutCartDomRes>.Success();
            }

            AboutCartDomRes userCartDetails = getCart.Entity.CheckCart();

            return EntityResult<AboutCartDomRes>.Success(userCartDetails);
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

        private CartResponse MapCartResponse(Cart cart)
        {
            return new CartResponse
            {
                CartId = cart.Id,
                CartLineResponse = _mapper.Map<IEnumerable<CartLineResponse>>(cart.CartLines),
            };
        }

        private static bool IsCreationDateActual(DateTime requestCartDate, DateTime dbCartDate)
        {
            return requestCartDate == dbCartDate;
        }
    }
}