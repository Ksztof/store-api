using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IGuestSessionService _guestSessionService;

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, IGuestSessionService guestSessionService)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _guestSessionService = guestSessionService;
        }

        public async Task<CartDto> AddProductToCartAsync(int productId, decimal productQuantity)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new EntityNotFoundException<Product, int>($"There is no Entity of type {typeof(Product)} with Id - {productId}");
            }

            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            Cart cart;
            if (getCartIdFromCookie != null)
            {
                cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart is null)
                {
                    throw new EntityNotFoundException<Product, int>($"The cart with Id: {getCartIdFromCookie.Value} was not found.");
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
                _guestSessionService.SendCartIdToGuest(cart.Id);
            }
            CartDto cartDto = MapCartDto(cart);

            return cartDto;
        }

        public async Task<CartDto> DeleteCartLineFromCartAsync(int productId)
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }

            CartLine? cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                throw new EntityNotFoundException<CartLine, int>($"There is no entity of type: {typeof(CartLine)} You're serching for a cart line that includes product of Id: {productId}");
            }

            await _cartsRepository.DeleteCartLineAsync(cartLine);
            cart.DeleteCartLineFromCart(productId);
            cart = await _cartsRepository.UpdateAsync(cart);
            CartDto cartDto = MapCartDto(cart);

            return cartDto;
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }

            return cart;
        }

        public async Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            cart.SetProductQuantity(productId, productQuantity);

            return cart;
        }

        public async Task<CheckCartForm> CheckCartAsync()
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }

            CheckCartForm informationAboutCart = cart.CheckCart();

            return informationAboutCart;
        }

        public async Task<Cart> ClearCartAsync()
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }

            cart.ClearCart();
            return cart;
        }

        private static CartDto MapCartDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                CartLineDto = cart.CartLines.Select(x => new CartLineDto
                {
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.Product.Price,
                    TotalPrice = x.Quantity * x.Product.Price,
                })
            };
        }
    }
}