using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.Results;

namespace PerfumeStore.Application.Carts
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly ICookiesService _cookiesService;
        private readonly IMapper _mapper;

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, ICookiesService guestSessionService, IMapper mapper)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cookiesService = guestSessionService;
            _mapper = mapper;
        }

        public async Task<Result<CartResponse>> AddProductsToCartAsync(AddProductsToCartDtoApplication request)
        {
            int[] newProductsIds = request.Products.Select(product => product.ProductId).ToArray();

            IEnumerable<Product> products = await _productsRepository.GetByIdsAsync(newProductsIds);

            int[] dbProductsIds = products.Select(x => x.Id).ToArray();

            if (newProductsIds.Count() != dbProductsIds.Count())
            {
                var missingIds = newProductsIds.Except(dbProductsIds).ToArray();
                return Result<CartResponse>.Failure(EntityErrors<Product, int>.MissingEntities(missingIds));
            }

            int? GuestCartId = _cookiesService.GetCartId();

            Cart? cart;
            AddProductsToCartDtoDomain addProductsToCartDtoDomain = _mapper.Map<AddProductsToCartDtoDomain>(request);
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
            if (GuestCartId == null)
            {
                return Result<CartResponse>.Failure(CookieError.MissingCookie());
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

            return Result<CartResponse>.Success();
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

            if (GuestCartId == null)
            {
                return Result<CartResponse>.Failure(CookieError.MissingCookie());
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
            }

            ModifyProductDtoDomain modifiedProductForDomain = _mapper.Map<ModifyProductDtoDomain>(productModification);

            cart.ModifyProduct(modifiedProductForDomain);

            cart = await _cartsRepository.UpdateAsync(cart);

            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<AboutCartRes>> CheckCartAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            if (GuestCartId == null)
            {
                return Result<AboutCartRes>.Failure(CookieError.MissingCookie());
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
            if (GuestCartId == null)
            {
                return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
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