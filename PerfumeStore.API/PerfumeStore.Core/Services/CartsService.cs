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
                    var carLineNewCart = new CartLine
                    {
                        CartId = newCart.Id,
                        ProductId = product.Id,
                        Quantity = productQuantity,
                    };
                    newCart.CartLines.Add(carLineNewCart);
                    _guestSessionService.SendCartId(newCart.Id);
                    newCart = await _cartsRepository.CreateAsync(newCart);

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
                IEnumerable<int> productsIds = cart.CartLines.Select(x => x.ProductId);
                decimal totalCartValue = 0;
                foreach (int productId in productsIds)
                {
                    Product product = await _productsRepository.GetByIdAsync(productId);
                    decimal productPrice = product.Price;
                    decimal productQuantity = cart.CartLines.SingleOrDefault(x => x.ProductId == productId).Quantity;
                    totalCartValue += productPrice * productQuantity;
                }
                ICollection<CheckCart> formattedCartProducts = new List<CheckCart>();
                foreach (CartLine cartLine in cart.CartLines)
                {
                    var product = await _productsRepository.GetByIdAsync(cartLine.ProductId);
                    var cartProduct = new CheckCart
                    {
                        ProductId = cartLine.ProductId,
                        ProductUnitPrice = product.Price,
                        ProductTotalPrice = product.Price * cartLine.Quantity,
                        Quantity = cartLine.Quantity,
                    };
                    formattedCartProducts.Add(cartProduct);
                }
                CheckCartForm formatedCart = new CheckCartForm
                {
                    TotalCartValue = totalCartValue,
                    ProductsInCart = formattedCartProducts,
                };

                return await Task.FromResult(formatedCart);
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
            cart.CartLine.Clear();
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(cart);
        }
    }
}