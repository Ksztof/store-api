using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.ResponseForms;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepository _cartsRepository;
		private readonly IProductsRepository _productsRepository;

		public CartsService(ICartsRepository cartsRepository, IProductsRepository productsRepository)
        {
            _cartsRepository = cartsRepository;
            _productsRepository = productsRepository;
        }


		public async Task<Cart?> AddProductToCartAsync(int productId, int userId, decimal productQuantity)
        {
            Cart? cart = await _cartsRepository.CheckIfCartExists(userId);
			Product product = await _productsRepository.GetByIdAsync(productId); //TODO: validation
            bool? isProductInCart = cart?.CartProducts?.ContainsKey(productId);
            var productWithQuantity= new CartProduct
            {
                Product = product,
                ProductQuantity = productQuantity,
            };

			if (cart == null)
            {
                var newCart = new Cart
                {
                    CartId = CartIdGenerator.GetNextId(),
                    CartProducts = new Dictionary<int, CartProduct> { { productId, productWithQuantity } }, //TODO: Add product with give quantity if same product is added "1 by 1" product in cart should increase his quantity instead of adding new product
                    UserId = userId,
                };
                newCart = await _cartsRepository.CreateAsync(newCart);
            }

            if (isProductInCart == true)
            {
                cart.CartProducts[productId].ProductQuantity += productQuantity;
			}
            else cart.CartProducts.Add(userId, productWithQuantity);

			Cart? updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
		}

        public async Task<Cart> DecreaseProductQuantityAsync(int productId, int userId)
        {
            Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
            cart.CartProducts[productId].ProductQuantity -= 1;
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

		public async Task<Cart> IncreaseProductQuantityAsync(int productId, int userId)
		{
			Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
			cart.CartProducts[productId].ProductQuantity += 1;
			Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

			return await Task.FromResult(updatedCart);
		}

		public async Task<Cart> DeleteProductLineFromCartAsync(int productId, int userId)
        {
            Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
            bool removingResult = cart.CartProducts.Remove(productId);
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
		{
			Cart cart = await _cartsRepository.GetByCartIdAsync(cartId);

			return await Task.FromResult(cart);
		}

        public async Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity, int userId)
        {
            Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
            cart.CartProducts[productId].ProductQuantity = productQuantity;
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(updatedCart);
        }

        public async Task<CheckCartForm> CheckCartAsync(int userId)
        {
            Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
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

        public async Task<Cart> ClearCartAsync(int userId)
        {
            Cart cart = await _cartsRepository.GetByUserIdAsync(userId);
            cart.CartProducts.Clear();
            Cart updatedCart = await _cartsRepository.UpdateAsync(cart);

            return await Task.FromResult(cart);
        }
    }
}
