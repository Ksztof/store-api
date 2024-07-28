using AutoMapper;
using PerfumeStore.Application.Contracts.ContextHttp;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.Shared.DTO.models;
using PerfumeStore.Domain.Shared.DTO.Request.Product;
using PerfumeStore.Domain.Shared.DTO.Response.Cart;
using PerfumeStore.Domain.Shared.Errors;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Carts
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

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null)
                {
                    userCart = new Cart { StoreUserId = userId };
                    userCart.AddProducts(newProductsIds);
                    userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);
                    userCart = await _cartsRepository.CreateAsync(userCart);
                }
                else
                {
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
                guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

                if (guestCart is null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
                }

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
                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null)
                {
                    userCart = new Cart { StoreUserId = userId };
                    userCart.AddProducts(newProductsIds);
                    userCart.UpdateProductsQuantity(addProductsToCartDtoDomain);
                    userCart = await _cartsRepository.CreateAsync(userCart);
                }
                else
                {
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
                guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

                if (guestCart is null)
                {
                    return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
                }

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

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.NotFound(productId));
                }

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

            Cart? guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
            }

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

            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(cartId));
            }

            CartResponse cartResponse = MapCartResponse(cart);

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

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(result.Value);

                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(productModification.Product.ProductId));
                }

                userCart.ModifyProduct(modifiedProductForDomain);
                userCart = await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
            }

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

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(result.Value);

                if (userCart == null || userCart.CartLines == null || !userCart.CartLines.Any())
                {
                    return EntityResult<AboutCartDomRes>.Success();
                }

                AboutCartDomRes userCartDetails = userCart.CheckCart();

                return EntityResult<AboutCartDomRes>.Success(userCartDetails);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (guestCart == null)
            {
                return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
            }

            AboutCartDomRes guestCartDetails = guestCart.CheckCart();

            return EntityResult<AboutCartDomRes>.Success(guestCartDetails);
        }

        public async Task<EntityResult<CartResponse>> ClearCartAsync()
        {
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

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(result.Value);
                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
                }

                ICollection<CartLine> userCartLines = userCart.CartLines;
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(receiveGuestCartIdResult.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(receiveGuestCartIdResult.Value));
            }

            ICollection<CartLine> cartLines = guestCart.CartLines;
            await _cartLinesRepository.ClearCartAsync(cartLines);
            guestCart.ClearCart();

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<CartResponse>> AssignGuestCartToUserAsync(string userId, int cartId)
        {
            Cart? guestCart = await _cartsRepository.GetByIdAsync(cartId);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.NotFound(cartId));
            }

            Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

            if (userCart == null)
            {
                guestCart.AssignUserToCart(userId);
                await _cartsRepository.UpdateAsync(guestCart);

                return EntityResult<CartResponse>.Success();
            }

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


                cartId = await _cartsRepository.GetCartIdByUserIdAsync(result.Value);
            }

            if (receiveGuestCartIdResult.IsSuccess)
            {
                cartId = receiveGuestCartIdResult.Value;
            }

            if (cartId > 0)
            {
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

                Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return EntityResult<AboutCartDomRes>.Success();
                }

                AboutCartDomRes userCartDetails = cart.CheckCart();

                return EntityResult<AboutCartDomRes>.Success(userCartDetails);
            }
            else
            {
                throw new InvalidOperationException("Cart ID was not properly initialized.");
            }
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