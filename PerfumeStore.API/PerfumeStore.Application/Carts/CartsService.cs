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
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.Results;
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

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, ICookiesService guestSessionService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHttpContextService httpContextService, UserManager<StoreUser> userManager)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cookiesService = guestSessionService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _httpContextService = httpContextService;
            _userManager = userManager;
        }

        public async Task<Result<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApplication request)
        {
            int? GuestCartId = _cookiesService.GetCartId();
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                return Result<CartResponse>.Failure(EntityErrors<Product, int>.MissingEntity(222222222));
            }

            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();

            IEnumerable<Product> products = await _productsRepository.GetByIdsAsync(newProductsIds);

            int[] dbProductsIds = products.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != dbProductsIds.Count())
            {
                var missingIds = newProductsIds.Except(dbProductsIds).ToArray();
                return Result<CartResponse>.Failure(EntityErrors<Product, int>.MissingEntities(missingIds));
            }

            AddProductsToCartDtoDomain addProductsToCartDtoDomain = _mapper.Map<AddProductsToCartDtoDomain>(request);

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

                return Result<CartResponse>.Success(userCartResponse);
            }

            Cart? cart;
            if (GuestCartId != null)
            {
                cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
                if (cart is null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<CartResponse>> DeleteCartLineFromCartAsync(int productId)
        {
            int? GuestCartId = _cookiesService.GetCartId();
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(productId));
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<CartLine, int>.MissingEntity(productId));
                }

                CartLine? userCartLine = userCart?.CartLines?.FirstOrDefault(cl => cl.ProductId == productId);
                if (userCartLine == null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<CartLine, int>.MissingEntity(productId)); 
                }

                userCart?.DeleteCartLineFromCart(productId);

                await _cartsRepository.UpdateAsync(userCart);//TODO: Check if need to use delete CartLine separately from DB

                CartResponse userCartResponse = MapCartResponse(userCart);

                return Result<CartResponse>.Success(userCartResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            CartLine? cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));//TODO: should return cartline Id not Cart because this cart exist 
            }

            await _cartsRepository.DeleteCartLineAsync(cartLine);
            cart.DeleteCartLineFromCart(productId);
            cart = await _cartsRepository.UpdateAsync(cart);
            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<CartResponse>> GetCartResponseByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
            }

            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<Cart>> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return Result<Cart>.Failure(EntityErrors<Cart, int>.MissingEntity(cartId));
            }

            return Result<Cart>.Success(cart);
        }

        public async Task<Result<CartResponse>> ModifyProductAsync(ModifyProductDtoApplication productModification)
        {
            int? GuestCartId = _cookiesService.GetCartId();
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(productModification.Product.ProductId));
            }

            ModifyProductDtoDomain modifiedProductForDomain = _mapper.Map<ModifyProductDtoDomain>(productModification);

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(productModification.Product.ProductId));
                }

                userCart.ModifyProduct(modifiedProductForDomain);

                userCart = await _cartsRepository.UpdateAsync(userCart);

                CartResponse userCartResponse = MapCartResponse(userCart);

                return Result<CartResponse>.Success(userCartResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            cart.ModifyProduct(modifiedProductForDomain);

            cart = await _cartsRepository.UpdateAsync(cart);

            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<AboutCartRes>> CheckCartAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                return Result<AboutCartRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return Result<AboutCartRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                AboutCartRes aboutUserCartResposne = userCart.CheckCart();

                return Result<AboutCartRes>.Success(aboutUserCartResposne);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return Result<AboutCartRes>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            AboutCartRes aboutCartResposne = cart.CheckCart();

            return Result<AboutCartRes>.Success(aboutCartResposne);
        }

        public async Task<Result<CartResponse>> ClearCartAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            if (GuestCartId == null && isUserAuthenticated == false)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                ICollection<CartLine> userCartLines = userCart.CartLines;
                userCart.ClearCart();
                await _cartsRepository.UpdateAsync(userCart);
                CartResponse userCartResponse = MapCartResponse(userCart);//TODO: _cartsRepository.ClearCartAsync(cartLines); or just update cart with cleard CartLines
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            ICollection<CartLine> cartLines = cart.CartLines;
            await _cartsRepository.ClearCartAsync(cartLines);
            cart.ClearCart();
            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
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