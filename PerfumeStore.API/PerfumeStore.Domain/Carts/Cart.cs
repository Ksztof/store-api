﻿using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Core.DTO;
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
        public ICollection<CartLine>? CartLines { get; set; } = new List<CartLine>();
        public string? UserId { get; set; }
        public StoreUser? User { get; set; }

        public void AddProduct(int productId)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine == null)
            {
                CartLine newLine = new CartLine
                {
                    ProductId = productId,
                };
                CartLines.Add(newLine);
            }
        }

        public void UpdateProductQuantity(int productId, decimal quantity)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(x => x.ProductId == productId);
            if (cartLine != null)
            {
                cartLine.Quantity += quantity;
            }
        }

        public void DeleteCartLineFromCart(int productId)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(x => x.ProductId == productId);
            bool deleteSuccess = CartLines.Remove(cartLine);
        }

        public void SetProductQuantity(int productId, decimal productQuantity)
        {
            CartLine? cartLine = CartLines.FirstOrDefault(x => x.ProductId == productId);
            cartLine.Quantity = productQuantity;
        }

        public void ClearCart()
        {
            CartLines.Clear();
        }

        public AboutCartRes CheckCart()
        {
            decimal totalCartValue = CartLines.Sum(x => x.Product.Price * x.Quantity);
            IEnumerable<CheckCartDto> aboutProducts = GetInformationAboutProducts();

            AboutCartRes aboutCart = new AboutCartRes
            {
                AboutProductsInCart = aboutProducts,
                TotalCartValue = totalCartValue
            };

            return aboutCart;
        }

        private IEnumerable<CheckCartDto> GetInformationAboutProducts()
        {
            return CartLines.Select(x => new CheckCartDto
            {
                ProductUnitPrice = x.Product.Price,
                ProductTotalPrice = x.Product.Price * x.Quantity,
                Quantity = x.Quantity,
            });
        }
    }
}