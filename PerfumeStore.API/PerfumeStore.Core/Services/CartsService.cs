using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Core.Services
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IGuestSessionService _guestSessionService;
        private readonly IValidationService _validationService;

        public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository, IGuestSessionService guestSessionService, IValidationService validationService)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
            _guestSessionService = guestSessionService;
            _validationService = validationService;
        }

        public async Task<CartResponse> AddProductToCartAsync(int productId, decimal productQuantity)
        {
            var quantityValidation = _validationService.ValidateQuantity(productQuantity);
            var idValidation = _validationService.ValidateEntityId(productId);
            if (!quantityValidation.IsValid && !idValidation.IsValid)
            {
                IEnumerable<string> idErrors = quantityValidation.Errors.Select(x => x.ErrorMessage).ToList();
                IEnumerable<string> quantityErrors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();

                throw new ValidationException($"Validation Details: id errors --- {idErrors} \n quantity errors --- {quantityErrors}");
            }

            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new EntityNotFoundException<Product, int>(product.Id);
            }

            int? GuestCartId = _guestSessionService.GetCartId();
            Cart? cart;
            if (GuestCartId != null)
            {
                cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
                if (cart is null)
                {
                    throw new EntityNotFoundException<Product, int>(cart.Id);
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
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<CartResponse> DeleteCartLineFromCartAsync(int productId)
        {
            var idValidation = _validationService.ValidateEntityId(productId);
            if (!idValidation.IsValid)
            {
                IEnumerable<string> errors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();
                throw new Exception($"Id is not correct. Details: {errors}");
            }

            int? GuestCartId = _guestSessionService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
            }

            CartLine? cartLine = cart.CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                throw new EntityNotFoundException<CartLine, int>(cartLine.Id);
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
                throw new EntityNotFoundException<Cart, int>(cart.Id);
            }

            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            Cart? cart = await _cartsRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
            }

            return cart;
        }

        public async Task<CartResponse> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            var quantityValidation = _validationService.ValidateQuantity(productQuantity);
            var idValidation = _validationService.ValidateEntityId(productId);
            if (!quantityValidation.IsValid && !idValidation.IsValid)
            {
                IEnumerable<string> idErrors = quantityValidation.Errors.Select(x => x.ErrorMessage).ToList();
                IEnumerable<string> quantityErrors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();

                throw new Exception($"Validation Details: id errors --- {idErrors} \n quantity errors --- {quantityErrors}");
            }

            int? GuestCartId = _guestSessionService.GetCartId();

            if (GuestCartId == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
            }

            cart.SetProductQuantity(productId, productQuantity);
            cart = await _cartsRepository.UpdateAsync(cart);
            CartResponse cartResponse = MapCartResponse(cart);

            return cartResponse;
        }

        public async Task<AboutCartResponse> CheckCartAsync()
        {
            int? GuestCartId = _guestSessionService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
            }

            AboutCartResponse aboutCartResposne = cart.CheckCart();

            return aboutCartResposne;
        }

        public async Task<CartResponse> ClearCartAsync()
        {
            int? GuestCartId = _guestSessionService.GetCartId();
            if (GuestCartId == null)
            {
                throw new MissingDataInCookieException($"Guest Cookie doesn't contain cart id. Value: {GuestCartId}");
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
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