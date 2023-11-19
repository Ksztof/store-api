﻿using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Application.CustomExceptions;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly ICookieService _guestSessionService;
        private readonly ICartsRepository _cartsRepository;

        public OrdersService(IOrdersRepository ordersRepository, ICookieService guestSessionService, ICartsRepository cartsRepository)
        {
            _ordersRepository = ordersRepository;
            _guestSessionService = guestSessionService;
            _cartsRepository = cartsRepository;
        }

        public async Task<OrderResponse> CreateOrderAsync()
        {
            int? GuestCartId = _guestSessionService.GetCartId();
            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundEx<Cart, int>(cart.Id);
            }

            Order order = new Order();
            order.CreateOrder(cart.Id);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartRes checkCart = cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, checkCart);

            return orderResponse;
        }

        public async Task<OrderResponse> GetByIdAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundEx<Order, int>(order.Id);
            }

            AboutCartRes aboutCart = order.Cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, aboutCart);

            return orderResponse;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundEx<Order, int>(order.Id);
            }

            await _ordersRepository.DeleteOrderAsync(order);
        }

        public async Task MarkOrderAsDeletedAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {S
                throw new EntityNotFoundEx<Order, int>(order.Id);
            }

            order.MarkAsDeleted();
            await _ordersRepository.UpdateAsync(order);
        }

        private static OrderResponse MapAboutCartToOrderRes(Order order, AboutCartRes checkCart)
        {
            return new OrderResponse
            {
                Id = order.Id,
                AboutProductsInCart = checkCart.AboutProductsInCart,
                TotalCartValue = checkCart.TotalCartValue,
            };
        }
    }
}