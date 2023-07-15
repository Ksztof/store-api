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
                Product? product = await _productsRepository.GetByIdAsync(productId); //TODO: validation
                if (product == null)
                {
                    throw new ProductNotFoundException($"There is no product with the given id. Id: {productId}");
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
                    newCart.CartLine.Add(carLineNewCart);
                    _guestSessionService.SendCartId(newCart.Id);
                    newCart = await _cartsRepository.CreateAsync(newCart);
                    return await Task.FromResult(newCart);
                }
                Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
                CartLine? cartLine = cart?.CartLine.SingleOrDefault(x => x.ProductId == productId);
                var cartLineOldCart = new CartLine
                {
                    Id = CartLineIdGenerator.GetNextId(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = productQuantity,
                };
                if (cartLine != null)
                {
                    cartLine.Quantity += productQuantity;
                    Cart? updatedcartLineQunatity = await _cartsRepository.UpdateAsync(cart);
                    return await Task.FromResult(updatedcartLineQunatity);
                }
                cart.CartLine.Add(cartLineOldCart);
                Cart? addedNewCartLine = await _cartsRepository.UpdateAsync(cart);

                return await Task.FromResult(addedNewCartLine);
            }
            catch (ProductNotFoundException e)
            {
                throw new ProductNotFoundException($"There is no product with the given id. Id: {productId}", e);
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
                Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
                if (cart == null)
                {
                    throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
                }
                CartLine? cartLine = cart?.CartLine.SingleOrDefault(x => x.ProductId == productId);
                cartLine.Quantity -= 1;
                Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

                return await Task.FromResult(updatedCart);
            }
            catch (CookieWithCartIdNotFoundException e)
            {
                throw new CookieWithCartIdNotFoundException("Guest Cookie doesn't contain cart id.", e);
            }
            catch (CartNotFoundException e)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id.", e);
            }
        }

        public async Task<Cart> IncreaseProductQuantityAsync(int productId)
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            CartLine? cartLine = cart?.CartLine.SingleOrDefault(x => x.ProductId == productId);
            cartLine.Quantity -= 1;
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

        public async Task<Cart> DeleteProductLineFromCartAsync(int productId)
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            CartLine? cartLine = cart?.CartLine.SingleOrDefault(x => x.ProductId == productId);
            cart.CartLine.Remove(cartLine);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByCartIdAsync(cartId);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            return await Task.FromResult(cart);
        }

        public async Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            CartLine? cartLine = cart?.CartLine.SingleOrDefault(x => x.ProductId == productId);
            cartLine.Quantity = productQuantity;
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

        public async Task<CheckCartForm> CheckCartAsync()
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
            if (cart == null)
            {
                throw new CartNotFoundException($"Cart id is present but there isn't cart with given cart id. Value: {cart}");
            }
            IEnumerable<int> productsIds = cart.CartLine.Select(x => x.ProductId);
            decimal totalCartValue = 0;
            foreach (int productId in productsIds)
            {
                Product product = await _productsRepository.GetByIdAsync(productId);
                decimal productPrice = product.Price;
                decimal productQuantity = cart.CartLine.SingleOrDefault(x => x.ProductId == productId).Quantity;
                totalCartValue += productPrice * productQuantity;
            }
            ICollection<CheckCart> formattedCartProducts = new List<CheckCart>();
            foreach (CartLine cartLine in cart.CartLine)
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

        public async Task<Cart> ClearCartAsync()
        {
            int? getCartIdFromCookie = _guestSessionService.GetCartId();
            if (getCartIdFromCookie == null)
            {
                throw new CookieWithCartIdNotFoundException($"Guest Cookie doesn't contain cart id. Value: {getCartIdFromCookie}");
            }
            Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
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