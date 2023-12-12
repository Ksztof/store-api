using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Carts
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IGuestSessionService _guestSessionService;
        private readonly IMapper _mapper;
        private readonly IHttpContextService _httpContextService;
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
            _httpContextService = httpContextService;
            _cartLinesRepository = cartLinesRepository;
        }

        public async Task<EntityResult<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApp request)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();

            int? GuestCartId = _guestSessionService.GetCartId();

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();

            IEnumerable<Product> products = await _productsRepository.GetByIdsAsync(newProductsIds);

            int[] dbProductsIds = products.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != dbProductsIds.Count())
            {
                var missingIds = newProductsIds.Except(dbProductsIds).ToArray();
                return EntityResult<CartResponse>.Failure(EntityErrors<Product, int>.MissingEntities(missingIds));
            }

            AddProductsToCartDtoDom addProductsToCartDtoDomain = _mapper.Map<AddProductsToCartDtoDom>(request);

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

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

                CartResponse userCartResponse = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartResponse);
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

            CartResponse cartResponse = MapCartResponse(guestCart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

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

                userCart?.DeleteCartLineFromCart(productId);

                await _cartLinesRepository.DeleteCartLineAsync(userCartLine);

                await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartResponse = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            CartLine? cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));//TODO: should return cartline Id not Cart because this cart exist 
            }

            cart.DeleteCartLineFromCart(productId);
            await _cartLinesRepository.DeleteCartLineAsync(cartLine);

            cart = await _cartsRepository.UpdateAsync(cart);
            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
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

        public async Task<EntityResult<Cart>> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return EntityResult<Cart>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
            }

            return EntityResult<Cart>.Success(cart);
        }

        public async Task<EntityResult<CartResponse>> ModifyProductAsync(ModifyProductDtoApp productModification)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            ModifyProductDtoDom modifiedProductForDomain = _mapper.Map<ModifyProductDtoDom>(productModification);

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(productModification.Product.ProductId));
                }

                userCart.ModifyProduct(modifiedProductForDomain);

                userCart = await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartResponse = MapCartResponse(userCart);

                return EntityResult<CartResponse>.Success(userCartResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            cart.ModifyProduct(modifiedProductForDomain);

            cart = await _cartsRepository.UpdateAsync(cart);

            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<AboutCartRes>> CheckCartAsync()
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<AboutCartRes>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null || userCart.CartLines == null || !userCart.CartLines.Any())
                {
                    return EntityResult<AboutCartRes>.Success();
                }

                AboutCartRes aboutUserCartResposne = userCart.CheckCart();

                return EntityResult<AboutCartRes>.Success(aboutUserCartResposne);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return EntityResult<AboutCartRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            AboutCartRes aboutCartResposne = cart.CheckCart();

            return EntityResult<AboutCartRes>.Success(aboutCartResposne);
        }

        public async Task<EntityResult<CartResponse>> ClearCartAsync()
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                ICollection<CartLine> userCartLines = userCart.CartLines;
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);
                CartResponse userCartResponse = MapCartResponse(userCart);//TODO: _cartsRepository.ClearCartAsync(cartLines); or just update cart with cleard CartLines

                return EntityResult<CartResponse>.Success(userCartResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            ICollection<CartLine> cartLines = cart.CartLines;
            await _cartLinesRepository.ClearCartAsync(cartLines);
            cart.ClearCart();
            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> AssignCartToUserAsync(string userId, int cartId)
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

                await _cartsRepository.UpdateAsync(guestCart); //TODO: check update operation on repo

                return EntityResult<CartResponse>.Success();
            }

            int[] newProductsIds = guestCart.CartLines.Select(cl => cl.ProductId).ToArray();

            AddProductsToCartDtoDom productsQuantity = new AddProductsToCartDtoDom
            {
                Products = guestCart.CartLines.Select(cl => new ProductInCartDom
                {
                    ProductId = cl.ProductId,
                    Quantity = cl.Quantity,
                }).ToArray()
            };

            userCart.AddProducts(newProductsIds);

            userCart.UpdateProductsQuantity(productsQuantity);

            await _cartsRepository.UpdateAsync(userCart);

            return EntityResult<CartResponse>.Success();
        }

        private static CartResponse MapCartResponse(Cart cart)
        {
            return new CartResponse
            {
                Id = cart.Id,
                CartLineResponse = cart.CartLines.Select(x => new CartLineResponse
                {
                    productId = x.ProductId,
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.Product.Price,
                    TotalPrice = x.Quantity * x.Product.Price,
                })
            };
        }
    }
}