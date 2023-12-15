using AutoMapper;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.ShippingDetails;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly IGuestSessionService _cookiesService;
        private readonly ICartsRepository _cartsRepository;
        private readonly IHttpContextService _httpContextService;
        private readonly IMapper _mapper;

        public OrdersService(
            IOrdersRepository ordersRepository,
            IGuestSessionService cookiesService,
            ICartsRepository cartsRepository,
            IHttpContextService httpContextService,
            IMapper mapper)
        {
            _ordersRepository = ordersRepository;
            _cookiesService = cookiesService;
            _cartsRepository = cartsRepository;
            _httpContextService = httpContextService;
            _mapper = mapper;
        }

        public async Task<EntityResult<OrderResponse>> CreateOrderAsync(CreateOrderDtoApp createOrderDtoApp)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<OrderResponse>.Failure(error);
            }

            ShippingDet shippingDetail = new ShippingDet();

            CreateOrderDtoDom createOrderDtoDom = _mapper.Map<CreateOrderDtoDom>(createOrderDtoApp);
            shippingDetail.CreateShippingDetail(createOrderDtoDom);
            ShippingDetilResponse shippingDetailsRes = _mapper.Map<ShippingDetilResponse>(shippingDetail);

            Order order = new Order();

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    var error = EntityErrors<Cart, int>.MissingEntity(userId);

                    return EntityResult<OrderResponse>.Failure(error);
                }


                order.CreateOrder(userCart.Id, userId, shippingDetail);

                order = await _ordersRepository.CreateOrderAsync(order);
                AboutCartRes userCartContent = userCart.CheckCart();
                OrderResponse userOrderResponse = MapAboutCartToOrderRes(order, userCartContent, shippingDetailsRes);

                userCart.StoreUserId = null;
                await _cartsRepository.UpdateAsync(userCart);

                return EntityResult<OrderResponse>.Success(userOrderResponse);
            }

            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                var error = EntityErrors<Cart, int>.MissingEntity(GuestCartId.Value);

                return EntityResult<OrderResponse>.Failure(error);
            }

            order.CreateOrder(cart.Id, shippingDetail);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartRes cartContent = cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, cartContent, shippingDetailsRes);

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
            ShippingDetilResponse shippingDetailsRes = _mapper.Map<ShippingDetilResponse>(order.ShippingDetail);
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, cartContent, shippingDetailsRes);

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
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            int? GuestCartId = _cookiesService.GetCartId();

            if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<OrderResponse>.Failure(error);
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var error = EntityErrors<Order, int>.MissingEntity(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();
                if (order.Cart.StoreUser.Id == userId)
                {
                    order.MarkAsDeleted();

                    await _ordersRepository.UpdateAsync(order);

                    return EntityResult<OrderResponse>.Success();
                }
            }

            if (order.Cart.Id == GuestCartId)
            {
                order.MarkAsDeleted();

                await _ordersRepository.UpdateAsync(order);

                return EntityResult<OrderResponse>.Success();
            }

            var cancelationOrderError = EntityErrors<Order, int>.EntityDoesntBelongToYou(orderId);

            return EntityResult<OrderResponse>.Failure(cancelationOrderError);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();

            /*if (GuestCartId == null && isUserAuthenticated == false)
            {
                Error error = AuthenticationErrors.MissingCartIdCookieUserNotAuthenticated;

                return EntityResult<OrderResponse>.Failure(error);
            }*/


            string userId = _httpContextService.GetUserId();

            IEnumerable<Order> userOrders = await _ordersRepository.GetByUserIdAsync(userId);
            return userOrders; //ThrowJsonException_SerializerCycleDetected

            /*foreach (Order order in userOrders)
            {

            }*/
        }

        private static OrderResponse MapAboutCartToOrderRes(Order order, AboutCartRes checkCart, ShippingDetilResponse shippingDetailsRes)
        {
            return new OrderResponse
            {
                Id = order.Id,
                AboutProductsInCart = checkCart.AboutProductsInCart,
                TotalCartValue = checkCart.TotalCartValue,
                ShippingDetil = shippingDetailsRes,
            };
        }
    }
}