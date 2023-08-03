using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Enums;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly IGuestSessionService _guestSessionService;
        private readonly ICartsRepository _cartsRepository;

        public OrdersService(IOrdersRepository ordersRepository, IGuestSessionService guestSessionService, ICartsRepository cartsRepository)
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
                throw new EntityNotFoundException<Cart, int>($"Entity of type: {typeof(Cart)} not found. Entity id: {GuestCartId.Value}");
            }

            Order order = new Order();
            order.CreateOrder(cart.Id);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartResponse checkCart = cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, checkCart);

            return orderResponse;
        }

        public async Task<OrderResponse> GetByIdAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>($"Can't find entity of type: {typeof(Order)}, Entity Id: {orderId}");
            }

            AboutCartResponse aboutCart = order.Cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, aboutCart);

            return orderResponse;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>($"Can't find entity of type: {typeof(Order)}, Entity Id: {orderId}");
            }

            await _ordersRepository.DeleteOrderAsync(order);
        }

        public async Task MarkOrderAsDeletedAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>($"Can't find entity of type: {typeof(Order)}, Entity Id: {orderId}");
            }

            order.MarkAsDeleted();
            await _ordersRepository.UpdateAsync(order);
        }

        private static OrderResponse MapAboutCartToOrderRes(Order order, AboutCartResponse checkCart)
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
