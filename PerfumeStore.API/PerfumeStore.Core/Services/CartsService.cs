using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.ResponseForms;
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

        public async Task<Cart?> AddProductToCartAsync(int productId, decimal productQuantity)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new EntityNotFoundException<Product, int>($"There is no Entity of type {typeof(Product)} with Id - {productId}");
            }

            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                Cart newCart = GenerateNewCart(productQuantity, product.Id);
                newCart = await _cartsRepository.CreateAsync(newCart);
                _guestSessionService.SendCartIdToGuest(newCart.Id);

                return newCart;
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart is null)
            {
                throw new EntityNotFoundException<Product, int>($"The cart with Id: {getCartIdFromCookie.Value} was not found.");
            }

            CartLine cartLine = GetCartLine(product.Id, cart);
            if (cartLine != null)
            {
                IncreaseProductQuantity(productQuantity, cartLine);
                Cart? extendedCart = await _cartsRepository.UpdateAsync(cart);

                return extendedCart;
            }
            AddNewCartLineToCart(productQuantity, product, cart);

            return cart;
        }

        public async Task<Cart> DecreaseProductQuantityAsync(int productId)
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

            CartLine? cartLine = GetCartLine(productId, cart);
            DecreaceProductQuantityByOne(cartLine);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return updatedCart;
        }

        public async Task<Cart> IncreaseProductQuantityAsync(int productId)
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

            CartLine? cartLine = GetCartLine(productId, cart);
            IncreaseProductQuantityByOne(cartLine);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return updatedCart;
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

            CartLine? cartLine = GetCartLine(productId, cart);
            await _cartsRepository.DeleteCartLineAsync(cartLine);
            cart.CartLines.Remove(cartLine);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return updatedCart;
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

            CartLine? cartLine = GetCartLine(productId, cart);
            SetProductQuantity(productQuantity, cartLine);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return updatedCart;
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

            CheckCartForm formatedCart = GetInformationAboutCart(cart);

            return formatedCart;
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

            ClearAllCartLines(cart);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);


            return cart;
        }

        private static void ClearAllCartLines(Cart? cart)
        {
            cart.CartLines.Clear();
        }

        private static void AddNewCartLineToCart(decimal productQuantity, Product? product, Cart? cart)
        {
            cart.CartLines.Add(
                            new CartLine
                            {
                                Product = product,
                                Quantity = productQuantity,
                            });
        }

        private static void IncreaseProductQuantity(decimal productQuantity, CartLine cartLine)
        {
            cartLine.Quantity += productQuantity;
        }

        private static CartLine? GetCartLine(int productId, Cart cart)
        {
            return cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
        }

        private static Cart GenerateNewCart(decimal productQuantity, int productId)
        {
            var newCart = new Cart();
            newCart.CartLines.Add(
                    new CartLine
                    {
                        ProductId = productId,
                        Quantity = productQuantity,
                    });

            return newCart;
        }

        private static void DecreaceProductQuantityByOne(CartLine? cartLine)
        {
            cartLine.Quantity -= 1;
        }

        private static void IncreaseProductQuantityByOne(CartLine? cartLine)
        {
            cartLine.Quantity += 1;
        }

        private static void SetProductQuantity(decimal productQuantity, CartLine? cartLine)
        {
            cartLine.Quantity = productQuantity;
        }

        private static CheckCartForm FormatInformationAboutCart(decimal totalCartValue, IEnumerable<CheckCart> cartLineAsCheckCart)
        {
            return new CheckCartForm
            {
                TotalCartValue = totalCartValue,
                ProductsInCart = cartLineAsCheckCart,
            };
        }

        private static IEnumerable<CheckCart> FormatCartLineToCheckCart(Cart? cart)
        {
            return cart.CartLines.Select(x => new CheckCart
            {
                ProductId = x.ProductId,
                ProductUnitPrice = x.Product.Price,
                ProductTotalPrice = x.Quantity * x.Product.Price,
                Quantity = x.Quantity
            }).ToList();
        }

        private static decimal CalculateTotalCartValue(Cart? cart)
        {
            return cart.CartLines.Sum(x => x.Product.Price * x.Quantity);
        }

        private static CheckCartForm GetInformationAboutCart(Cart? cart)
        {
            decimal totalCartValue = 0;
            totalCartValue = CalculateTotalCartValue(cart);
            IEnumerable<CheckCart> cartLineAsCheckCart = FormatCartLineToCheckCart(cart);
            CheckCartForm formatedCart = FormatInformationAboutCart(totalCartValue, cartLineAsCheckCart);

            return formatedCart;
        }
    }
}