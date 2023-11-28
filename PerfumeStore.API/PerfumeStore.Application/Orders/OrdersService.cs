using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Orders;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly ICookiesService _cookiesService;
        private readonly ICartsRepository _cartsRepository;

        public OrdersService(IOrdersRepository ordersRepository, ICookiesService _cookiesService, ICartsRepository cartsRepository)
        {
            _ordersRepository = ordersRepository;
            _cookiesService = _cookiesService;
            _cartsRepository = cartsRepository;
        }

        public async Task<OrderResponse> CreateOrderAsync()
        {
            int? GuestCartId = _cookiesService.GetCartId();
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
            {
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