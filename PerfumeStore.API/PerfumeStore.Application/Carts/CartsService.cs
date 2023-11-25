using Microsoft.AspNetCore.Authorization;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
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

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, ICookiesService guestSessionService)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _cookiesService = guestSessionService;
        }

        public async Task<Result<CartResponse>> AddProductsToCartAsync(AddProductsToCartRequest request)
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

            Dictionary<int, decimal> productsWithQuantity = request.Products
                .ToDictionary(product => product.ProductId, x => x.Quantity);
            Cart? cart;
            if (GuestCartId != null)
            {
                cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
                if (cart is null)
                {
                    return Result<CartResponse>.Failure(EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value));
                }

                cart.AddProduct(newProductsIds);
                cart.UpdateProductQuantity(productsWithQuantity);
                cart = await _cartsRepository.UpdateAsync(cart);
            }
            else
            {
                cart = new Cart();
                cart.AddProduct(newProductsIds);
                cart.UpdateProductQuantity(productsWithQuantity);
                cart = await _cartsRepository.CreateAsync(cart);
                _cookiesService.SendCartIdToGuest(cart.Id);
            }
            CartResponse cartResponse = MapCartResponse(cart);

            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<CartResponse> DeleteCartLineFromCartAsync(int productId)
        {
            int? GuestCartId = _cookiesService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieEx($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            CartLine? cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                throw new EntityNotFoundEx<CartLine, int>(cartLine.Id);
            }

            await _cartsRepository.DeleteCartLineAsync(cartLine);
            cart.DeleteCartLineFromCart(productId);
            cart = await _cartsRepository.UpdateAsync(cart);
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<CartResponse> GetCartResponseByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            return cart;
        }

        public async Task<CartResponse> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null)
            {
                throw new MissingDataInCookieEx($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            cart.SetProductQuantity(productId, productQuantity);
            cart = await _cartsRepository.UpdateAsync(cart);
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<AboutCartRes> CheckCartAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieEx($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            AboutCartRes aboutCartResposne = cart.CheckCart();

            return aboutCartResposne;
        }

        public async Task<CartResponse> ClearCartAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieEx($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            ICollection<CartLine> cartLines = cart.CartLines;
            await _cartsRepository.ClearCartAsync(cartLines);
            cart.ClearCart();
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
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