﻿using PerfumeStore.Domain.DTO.Request.Product;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.CarLines;
using PerfumeStore.Domain.Entities.StoreUsers;
using PerfumeStore.Domain.Repositories.Generics;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Entities.Carts
{
    public class Cart : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public List<CartLine>? CartLines { get; set; } = new List<CartLine>();
        public string? StoreUserId { get; set; }
        public StoreUser? StoreUser { get; set; }
        public CartStatus CartStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public Cart()
        {
            CartStatus = CartStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddProducts(int[] productsIdsRequest)
        {
            int[] newProductIds = GetNewProductsIds(productsIdsRequest);

            IEnumerable<CartLine> newCartLines = BuildNewCartLines(newProductIds);

            CartLines.AddRange(newCartLines);
        }

        public void ReplaceProducts(int[] productsIdsRequest)
        {
            IEnumerable<CartLine> newCartLines = BuildNewCartLines(productsIdsRequest);
            CartLines?.Clear();
            CartLines?.AddRange(newCartLines);
        }


        public void UpdateProductsQuantity(NewProductsDtoDom productsWithQuantities)
        {
            foreach (var productWithQuantity in productsWithQuantities.Products)
            {
                CartLine? cartLine = CartLines?.FirstOrDefault(cl => cl.ProductId == productWithQuantity.ProductId);
                if(cartLine != null)
                    cartLine.Quantity += productWithQuantity.Quantity;
            }
        }

        public void DeleteCartLineFromCart(int productId)
        {
            CartLine? cartLine = CartLines.First(cl => cl.ProductId == productId);

            bool deleteSuccess = CartLines.Remove(cartLine);
        }

        public void ModifyProduct(ModifyProductDtoDom productModification)
        {
            CartLine? cartLine = CartLines.First(cl => cl.ProductId == productModification.Product.ProductId);

            cartLine.Quantity = productModification.Product.Quantity;
        }

        public void ClearCart()
        {
            CartLines.Clear();
        }

        public AboutCartDomRes CheckCart()
        {
            decimal totalCartValue = CartLines.Sum(cl => cl.Product.Price * cl.Quantity);
            IEnumerable<CheckCartDomRes> aboutProducts = GetInformationAboutProducts();

            AboutCartDomRes aboutCart = new AboutCartDomRes
            {
                AboutProductsInCart = aboutProducts,
                TotalCartValue = totalCartValue,
                CreatedAt = this.CreatedAt, 
            };

            return aboutCart;
        }

        public void AssignUserToCart(string userId)
        {
            StoreUserId = userId;
        }

        private IEnumerable<CheckCartDomRes> GetInformationAboutProducts()
        {
            return CartLines.Select(cl => new CheckCartDomRes
            {
                ProductId = cl.ProductId,
                ProductName = cl.Product.Name,
                ProductUnitPrice = cl.Product.Price,
                Description = cl.Product.Description,
                Manufacturer = cl.Product.Manufacturer,
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