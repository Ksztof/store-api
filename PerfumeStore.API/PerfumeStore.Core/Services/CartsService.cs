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
            try
            {
                Product? product = await _productsRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new ProductNotFoundException($"There is no product with the given id. ProductId: {productId}");
                }
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    var newCart = new Cart
                    {
                    };
                    newCart = await _cartsRepository.CreateAsync(newCart);

                    var carLineNewCart = new CartLine
                    {
                        CartId = newCart.Id,
                        ProductId = product.Id,
                        Quantity = productQuantity,
                    };
                    newCart.CartLines?.Add(carLineNewCart);
                    newCart = await _cartsRepository.CreateAsync(newCart);
                    _guestSessionService.SendCartId(newCart.Id);

                    return newCart;
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart is null)
                {
                    throw new CartNotFoundException($"The cart with Id: {getCartIdFromCookie.Value} was not found.");
                }

                var cartLineForOldCart = new CartLine
                {
                    CartId = cart.Id,
                    Product = product,
                    Quantity = productQuantity,
                };
                if (cart.CartLines.Any(x => x.ProductId == product.Id))
                {
                    CartLine cartLine = cart.CartLines.SingleOrDefault(x => x.ProductId == product.Id);
                    cartLine.Quantity += productQuantity;
                    Cart? updatedCart = await _cartsRepository.UpdateAsync(cart);

                    return updatedCart;
                }
                cart.CartLines.Add(cartLineForOldCart);
                Cart? extendedCart = await _cartsRepository.UpdateAsync(cart);

                return extendedCart;
            }
            catch (ProductNotFoundException e)
            {
                throw new ProductNotFoundException(e.Message, e);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e);
            }
        }


        public async Task<Cart> DecreaseProductQuantityAsync(int productId)
        {
            try
            {
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
                cartLine.Quantity -= 1;
                Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

                return await Task.FromResult(updatedCart);
            }
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException(e.Message, e);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e);
            }
        }

        public async Task<Cart> IncreaseProductQuantityAsync(int productId)
        {
            try
            {
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
                cartLine.Quantity += 1;
                Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

                return updatedCart;
            }
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException(e.Message, e);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e);
            }
        }

        public async Task<Cart> DeleteProductLineFromCartAsync(int productId)
        {
            try
            {
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
                cart.CartLines.Remove(cartLine);
                Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

                return updatedCart;
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e);
            }
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException(e.Message, e);
            }
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            try
            {
                Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                return cart;
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e);
            }
        }

        public async Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            try
            {
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                CartLine? cartLine = cart?.CartLines.SingleOrDefault(x => x.ProductId == productId);
                cartLine.Quantity = productQuantity;
                Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

                return updatedCart;
            }
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException(e.Message, e.InnerException);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e.InnerException);
            }
        }

        public async Task<CheckCartForm> CheckCartAsync()
        {
            try
            {
                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
                }
                Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
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
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException(e.Message, e.InnerException);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException(e.Message, e.InnerException);
            }
        }

        public async Task<Cart> ClearCartAsync()
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            cart.CartLines.Clear();
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return cart;
        }
    }
}