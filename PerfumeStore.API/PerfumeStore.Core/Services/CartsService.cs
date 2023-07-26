using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Enums;
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

        public async Task<Cart?> AddProductToCartAsync(int productId, decimal productQuantity)
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

            return cart;
        }
        
        public async Task<Cart> DeleteProductLineFromCartAsync(int productId)
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

            cart.DeleteProductLineFromCart(productId);
            cart = await _cartsRepository.UpdateAsync(cart);

            return cart;
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)// ??? I think it;s ok, there s no need to move it to Cart.cs????
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
    }
}