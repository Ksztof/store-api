using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.ResponseForms;
using PerfumeStore.Domain;
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
                    throw new ProductNotFoundException($"There is no product with the given id. ProductId: {productId}");
                }
                var productWithQuantity = new CartProduct
                {
                    Product = product,
                    ProductQuantity = productQuantity,
                };

                int? getCartIdFromCookie = _guestSessionService.GetCartId();
                if (getCartIdFromCookie == null)
                {
                    var newCart = new Cart
                    {
                        CartId = CartIdGenerator.GetNextId(),
                        CartProducts = new Dictionary<int, CartProduct> { { productId, productWithQuantity } },
                    };
                    _guestSessionService.SendCartId(newCart.CartId);
                    newCart = await _cartsRepository.CreateAsync(newCart);

                    return await Task.FromResult(newCart);
                }

                Cart? cart = await _cartsRepository.GetByCartIdAsync(getCartIdFromCookie.Value);
                bool? isProductInCart = cart?.CartProducts?.ContainsKey(productId);

                if (isProductInCart == true)
                {
                    cart.CartProducts[productId].ProductQuantity += productQuantity;
                }
                else cart.CartProducts.Add(productId, productWithQuantity);
                Cart? updatedCart = await _cartsRepository.UpdateAsync(cart);

                return await Task.FromResult(updatedCart);
            }
            catch (ProductNotFoundException e)
            {
                throw new ProductNotFoundException($"There is no product with the given id. ProductId: {productId}", e);
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
                cart.CartProducts[productId].ProductQuantity -= 1;
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
            cart.CartProducts[productId].ProductQuantity += 1;
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
            bool removingResult = cart.CartProducts.Remove(productId);
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
            cart.CartProducts[productId].ProductQuantity = productQuantity;
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
            decimal totalCartValue = cart.CartProducts.Sum(x => x.Value.ProductQuantity * x.Value.Product.Price);
            IEnumerable<CheckCart> formattedCartProducts = cart.CartProducts.Select(x => new CheckCart
            {
                ProductId = x.Key,
                ProductUnitPrice = x.Value.Product.Price,
                ProductTotalPrice = x.Value.Product.Price * x.Value.ProductQuantity,
                Quantity = x.Value.ProductQuantity
            });

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
            cart.CartProducts.Clear();
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(cart);
        }
    }
}