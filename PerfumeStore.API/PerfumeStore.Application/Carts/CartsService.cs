using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Carts
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly ICookiesService _cookiesService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpContextService _httpContextService;
        private readonly UserManager<StoreUser> _userManager;

        public CartsService(
            ICartsRepository cartsRepository,
            IProductsRepository productsRepository,
            ICookiesService guestSessionService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IHttpContextService httpContextService,
            UserManager<StoreUser> userManager)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cookiesService = guestSessionService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _httpContextService = httpContextService;
            _userManager = userManager;
        }

        public async Task<EntityResult<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApp request)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _cookiesService.GetCartId();
            
            if (GuestCartId == null && isUserAuthenticated == false) 
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

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
                string userId = _httpContextService.GetUserNameIdentifierClaim();

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

            Cart? cart;
            if (GuestCartId != null)
            {
                cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
                if (cart is null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                cart.AddProducts(newProductsIds);
                cart.UpdateProductsQuantity(addProductsToCartDtoDomain);
                cart = await _cartsRepository.UpdateAsync(cart);
            }
            else
            {
                cart = new Cart();
                cart.AddProducts(newProductsIds);
                cart.UpdateProductsQuantity(addProductsToCartDtoDomain);
                cart = await _cartsRepository.CreateAsync(cart);
                _cookiesService.SendCartIdToGuest(cart.Id);
            }

            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> DeleteCartLineFromCartAsync(int productId)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

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

                await _cartsRepository.UpdateAsync(userCart);//TODO: Check if need to use delete CartLine separately from DB

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

            await _cartsRepository.DeleteCartLineAsync(cartLine);
            cart.DeleteCartLineFromCart(productId);
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
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            ModifyProductDtoDom modifiedProductForDomain = _mapper.Map<ModifyProductDtoDom>(productModification);

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

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
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<AboutCartRes>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return EntityResult<AboutCartRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<CartResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                ICollection<CartLine> userCartLines = userCart.CartLines;
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);
                CartResponse userCartResponse = MapCartResponse(userCart);//TODO: _cartsRepository.ClearCartAsync(cartLines); or just update cart with cleard CartLines
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            ICollection<CartLine> cartLines = cart.CartLines;
            await _cartsRepository.ClearCartAsync(cartLines);
            cart.ClearCart();
            CartResponse cartResponse = MapCartResponse(cart);

            return EntityResult<CartResponse>.Success(cartResponse);
        }

        public async Task<EntityResult<CartResponse>> AssignCartToUserAsync(string userId, int cartId)
        {
            Cart? userCart = await _cartsRepository.GetByIdAsync(cartId);
            if (userCart == null) 
            {
                return EntityResult<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
            }

            userCart.AssignUserToCart(userId);

            await _cartsRepository.UpdateAsync(userCart); //TODO: check update operation on repo

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