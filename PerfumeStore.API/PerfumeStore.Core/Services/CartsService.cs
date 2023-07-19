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
                throw new EntityNotFoundException<Product, int>($"There is no product with the given id. ProductId: {productId}");
            }
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                var newCart = new Cart
                {
                    CartLines = new List<CartLine>()
                    {
                        new CartLine
                        {
                            ProductId = product.Id,
                            Quantity = productQuantity,
                        }
                    }
                };

                newCart = await _cartsRepository.CreateAsync(newCart);
                _guestSessionService.SendCartId(newCart.Id);

                return newCart;
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart is null)
            {
                throw new EntityNotFoundException<Product, int>($"The cart with Id: {getCartIdFromCookie.Value} was not found.");
            }

            CartLine cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == product.Id);
            if (cartLine != null)
            {
                cartLine.Quantity += productQuantity;
            }
            else
            {
                cart.CartLines.Add(
                new CartLine
                {
                    Product = product,
                    Quantity = productQuantity,
                });
            }

            Cart? extendedCart = await _cartsRepository.UpdateAsync(cart);

            return extendedCart;
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

            CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
            cartLine.Quantity -= 1;
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
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

            CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
            cartLine.Quantity += 1;
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

            CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
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

            CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
            cartLine.Quantity = productQuantity;
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

            decimal totalCartValue = 0;
            totalCartValue = cart.CartLines.Sum(x => x.Product.Price * x.Quantity);
            IEnumerable<CheckCart> cartLineAsCheckCart = cart.CartLines.Select(x => new CheckCart
            {
                ProductId = x.ProductId,
                ProductUnitPrice = x.Product.Price,
                ProductTotalPrice = x.Quantity * x.Product.Price,
                Quantity = x.Quantity
            }).ToList();

            CheckCartForm formatedCart = new CheckCartForm
            {
                TotalCartValue = totalCartValue,
                ProductsInCart = cartLineAsCheckCart,
            };

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

            cart.CartLines.Clear();
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return cart;
        }
    }
}