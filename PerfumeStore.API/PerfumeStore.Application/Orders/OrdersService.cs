using Microsoft.AspNetCore.Identity;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.Results;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly ICookiesService _cookiesService;
        private readonly ICartsRepository _cartsRepository;

        public OrdersService(IOrdersRepository ordersRepository, ICookiesService cookiesService, ICartsRepository cartsRepository)
        {
            _ordersRepository = ordersRepository;
            _cookiesService = cookiesService;
            _cartsRepository = cartsRepository;
        }

        public async Task<EntityResult<OrderResponse>> CreateOrderAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                var error = EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value);

                return EntityResult<OrderResponse>.Failure(error);
            }

            Order order = new Order();
            order.CreateOrder(cart.Id);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartRes checkCart = cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, checkCart);

            return EntityResult<OrderResponse>.Success(orderResponse);
        }

        public async Task<EntityResult<OrderResponse>> GetByIdAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var error = EntityErrors<Order, int>.MissingEntity(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            AboutCartRes aboutCart = order.Cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, aboutCart);

            return EntityResult<OrderResponse>.Success(orderResponse);
        }

        public async Task<EntityResult<OrderResponse>> DeleteOrderAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var error = EntityErrors<Order, int>.MissingEntity(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            await _ordersRepository.DeleteOrderAsync(order);

            return EntityResult<OrderResponse>.Success();
        }

        public async Task<EntityResult<OrderResponse>> MarkOrderAsDeletedAsync(int orderId)
        {
            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var error = EntityErrors<Order, int>.MissingEntity(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            order.MarkAsDeleted();

            await _ordersRepository.UpdateAsync(order);

            return EntityResult<OrderResponse>.Success();
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