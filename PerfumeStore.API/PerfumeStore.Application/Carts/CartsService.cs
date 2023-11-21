using Microsoft.AspNetCore.Authorization;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Products;

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

        public async Task<CartResponse> AddProductToCartAsync(int productId, decimal productQuantity)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new EntityNotFoundEx<Product, int>(product.Id);
            }

            int? GuestCartId = _cookiesService.GetCartId();
            Cart? cart;
            if (GuestCartId != null)
            {
                cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
                if (cart is null)
                {
                    throw new EntityNotFoundEx<Product, int>(cart.Id);
                }

                cart.AddProduct(productId);
                cart.UpdateProductQuantity(productId, productQuantity);
                cart = await _cartsRepository.UpdateAsync(cart);
            }
            else
            {
                cart = new Cart();
                cart.AddProduct(productId);
                cart.UpdateProductQuantity(productId, productQuantity);
                cart = await _cartsRepository.CreateAsync(cart);
                _cookiesService.SendCartIdToGuest(cart.Id);
            }
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
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