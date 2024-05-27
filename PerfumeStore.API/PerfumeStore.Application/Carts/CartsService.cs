using AutoMapper;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.models;
using PerfumeStore.Domain.DTO.Request.Product;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.CarLines;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Repositories;

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
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();
            IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);
            int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != existingProductsIds.Count())
            {
                int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

                return EntityResult<CartResponse>.Failure(EntityErrors<Product, int>.MissingEntities(missingProdIds));
            }

            NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
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

            if (GuestCartId != null)
            {
                guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

                if (guestCart is null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();
            IEnumerable<Product> newProducts = await _productsRepository.GetByIdsAsync(newProductsIds);
            int[] existingProductsIds = newProducts.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != existingProductsIds.Count())
            {
                int[] missingProdIds = newProductsIds.Except(existingProductsIds).ToArray();

                return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Product, int>.MissingEntities(missingProdIds));
            }

            NewProductsDtoDom addProductsToCartDtoDomain = _mapper.Map<NewProductsDtoDom>(request);

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
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

            if (GuestCartId != null)
            {
                guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

                if (guestCart is null)
                {
                    return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.MissingEntity(productId));
                }

                CartLine? userCartLine = userCart?.CartLines?.FirstOrDefault(cl => cl.ProductId == productId);

                if (userCartLine == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.MissingEntity(productId));
                }

                userCart.DeleteCartLineFromCart(productId);
                await _cartLinesRepository.DeleteCartLineAsync(userCartLine);
                await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            CartLine? cartLine = guestCart.CartLines.FirstOrDefault(x => x.ProductId == productId);

            if (cartLine == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<CartLine, int>.MissingEntityByProductId(productId));
            }

            guestCart.DeleteCartLineFromCart(productId);
            await _cartLinesRepository.DeleteCartLineAsync(cartLine);
            guestCart = await _cartsRepository.UpdateAsync(guestCart);

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<CartResponse>> GetCartResponseByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
            }

            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification)
        {
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            ModifyProductDtoDom modifiedProductForDomain = _mapper.Map<ModifyProductDtoDom>(productModification);

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(productModification.Product.ProductId));
                }

                userCart.ModifyProduct(modifiedProductForDomain);
                userCart = await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            guestCart.ModifyProduct(modifiedProductForDomain);
            guestCart = await _cartsRepository.UpdateAsync(guestCart);

            CartResponse guestCartContents = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(guestCartContents);
        }

        public async Task<EntityResult<AboutCartDomRes>> CheckCartAsync()
        {
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return EntityResult<AboutCartDomRes>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);

                if (userCart == null || userCart.CartLines == null || !userCart.CartLines.Any())
                {
                    return EntityResult<AboutCartDomRes>.Success();
                }

                AboutCartDomRes userCartDetails = userCart.CheckCart();

                return EntityResult<AboutCartDomRes>.Success(userCartDetails);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

            if (guestCart == null)
            {
                return EntityResult<AboutCartDomRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            AboutCartDomRes guestCartDetails = guestCart.CheckCart();

            return EntityResult<AboutCartDomRes>.Success(guestCartDetails);
        }

        public async Task<EntityResult<CartResponse>> ClearCartAsync()
        {
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                ICollection<CartLine> userCartLines = userCart.CartLines;
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartContents = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartContents);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);

            if (guestCart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
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
            bool isUserAuthenticated = _contextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();
            int cartId = int.MinValue;
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdOrUserCookieNotAuthenticated;

                return EntityResult<AboutCartDomRes>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _contextService.GetUserId();
                cartId = await _cartsRepository.GetCartIdByUserIdAsync(userId);
            }
            else if (GuestCartId != null)
            {
                cartId = GuestCartId.Value;
            }

            if (cartId > int.MinValue)
            {
                DateTime createdAtDb = await _cartsRepository.GetCartDateByIdAsync(cartId);

                bool isCartActual = IsCreationDateActual(addProductToCartDto.CreatedAt, createdAtDb);

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