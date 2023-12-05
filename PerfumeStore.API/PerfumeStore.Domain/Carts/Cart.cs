using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.StoreUsers;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Carts
{
    public class Cart : IEntity<int> //: interface?
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public List<CartLine>? CartLines { get; set; } = new List<CartLine>();
        public string? StoreUserId { get; set; }
        public StoreUser? StoreUser { get; set; }

        public void AddProducts(int[] productsIdsRequest)
        {
            int[] newProductIds = GetNewProductsIds(productsIdsRequest);

            IEnumerable<CartLine> newCartLines = BuildNewCartLines(newProductIds);

            CartLines.AddRange(newCartLines);
        }

        public void UpdateProductsQuantity(AddProductsToCartDtoDom productsWithQuantities)
        {
            foreach (var productWithQuantity in productsWithQuantities.Products)
            {
                CartLine cartLine = CartLines.First(cl => cl.ProductId == productWithQuantity.ProductId);
                cartLine.Quantity += productWithQuantity.Quantity;
            }
        }

        public void DeleteCartLineFromCart(int productId)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(cl => cl.ProductId == productId);

            bool deleteSuccess = CartLines.Remove(cartLine);
        }

        public void ModifyProduct(ModifyProductDtoDom productModification)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(cl => cl.ProductId == productModification.Product.ProductId);

            cartLine.Quantity = productModification.Product.Quantity;
        }

        public void ClearCart()
        {
            CartLines.Clear();
        }

        public AboutCartRes CheckCart()
        {
            decimal totalCartValue = CartLines.Sum(cl => cl.Product.Price * cl.Quantity);
            IEnumerable<CheckCartDto> aboutProducts = GetInformationAboutProducts();

            AboutCartRes aboutCart = new AboutCartRes
            {
                AboutProductsInCart = aboutProducts,
                TotalCartValue = totalCartValue
            };

            return aboutCart;
        }

        public void AssignUserToCart(string userId)
        {
            StoreUserId = userId;
        }

        private IEnumerable<CheckCartDto> GetInformationAboutProducts()
        {
            return CartLines.Select(cl => new CheckCartDto
            {
                ProductUnitPrice = cl.Product.Price,
                ProductTotalPrice = cl.Product.Price * cl.Quantity,
                Quantity = cl.Quantity,
            });
        }

        private static IEnumerable<CartLine> BuildNewCartLines(int[] newProductIds)
        {
            return newProductIds
                .Select(newProductId => new CartLine
                {
                    ProductId = newProductId
                });
        }

        private int[] GetNewProductsIds(int[] productsIdsRequest)
        {
            return productsIdsRequest
                .Except(CartLines.Select(cartline => cartline.ProductId))
                .ToArray();
        }
    }
}