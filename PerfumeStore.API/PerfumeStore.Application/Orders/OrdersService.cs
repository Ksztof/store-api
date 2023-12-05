using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.Orders;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly IGuestSessionService _cookiesService;
        private readonly ICartsRepository _cartsRepository;
        private readonly IHttpContextService _httpContextService;

        public OrdersService(
            IOrdersRepository ordersRepository,
            IGuestSessionService cookiesService,
            ICartsRepository cartsRepository,
            IHttpContextService httpContextService)
        {
            _ordersRepository = ordersRepository;
            _cookiesService = cookiesService;
            _cartsRepository = cartsRepository;
            _httpContextService = httpContextService;
        }

        public async Task<EntityResult<OrderResponse>> CreateOrderAsync()
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<OrderResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserNameIdentifierClaim();
                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    var error = EntityErrors<Cart, int>.MissingEntity(userId);

                    return EntityResult<OrderResponse>.Failure(error);
                }

                Order userOrder = new Order();
                userOrder.CreateOrder(userCart.Id);
                userOrder = await _ordersRepository.CreateOrderAsync(userOrder);
                AboutCartRes userCartContent = userCart.CheckCart();
                OrderResponse userOrderResponse = MapAboutCartToOrderRes(userOrder, userCartContent);

                return EntityResult<OrderResponse>.Success(userOrderResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                var error = EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value);

                return EntityResult<OrderResponse>.Failure(error);
            }

            Order order = new Order();
            order.CreateOrder(cart.Id);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartRes cartContent = cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, cartContent);

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

            AboutCartRes cartContent = order.Cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, cartContent);

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