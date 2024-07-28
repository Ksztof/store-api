using AutoMapper;
using PerfumeStore.Application.Abstractions;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Contracts.Email;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.Request.Order;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.Orders;
using PerfumeStore.Domain.Repositories;

namespace PerfumeStore.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly ICartsRepository _cartsRepository;
        private readonly IHttpContextService _httpContextService;
        private readonly IMapper _mapper;
        private readonly IGuestSessionService _guestSessionService;
        private readonly IEmailService _emailService;

        public OrdersService(
            IOrdersRepository ordersRepository,
            IGuestSessionService cookiesService,
            ICartsRepository cartsRepository,
            IHttpContextService httpContextService,
            IMapper mapper,
            IGuestSessionService guestSessionService,
            IEmailService emailService)
        {
            _ordersRepository = ordersRepository;
            _cartsRepository = cartsRepository;
            _httpContextService = httpContextService;
            _mapper = mapper;
            _guestSessionService = guestSessionService;
            _emailService = emailService;
        }

        public async Task<EntityResult<OrderResponse>> CreateOrderAsync(string? method, CreateOrderDtoApp createOrderDtoApp)
        {
            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated == false)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<OrderResponse>.Failure(error);
            }

            ShippingDet shippingDetail = new ShippingDet();
            CreateOrderDtoDom createOrderDtoDom = _mapper.Map<CreateOrderDtoDom>(createOrderDtoApp);
            shippingDetail.CreateShippingDetail(createOrderDtoDom);
            ShippingDetailResponse shippingDetailsRes = _mapper.Map<ShippingDetailResponse>(shippingDetail);

            Order order = new Order();

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();

                Cart? userCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (userCart == null)
                {
                    Error error = EntityErrors<Cart, int>.NotFoundByUserId(userId);

                    return EntityResult<OrderResponse>.Failure(error);
                }

                Order? userOrder = await _ordersRepository.GetByCartIdAsync(userCart.Id);

                if (userOrder != null)
                {
                    Error error = EntityErrors<Order, int>.EntityInUse(userOrder.Id, userCart.Id);

                    return EntityResult<OrderResponse>.Failure(error);
                }

                order.CreateOrder(userCart.Id, userId, shippingDetail);
                order = await _ordersRepository.CreateOrderAsync(order);

                userCart.StoreUserId = null;
                userCart.CartStatus = CartStatus.Archive;
                await _cartsRepository.UpdateAsync(userCart);

                AboutCartDomRes userCartContent = userCart.CheckCart();
                OrderResponse userOrderContent = MapAboutCartToOrderRes(order, userCartContent, shippingDetailsRes);

                await _emailService.SendOrderSummary(userOrderContent);

                return EntityResult<OrderResponse>.Success(userOrderContent);
            }

            int guestCartId = receiveCartIdResult.Value;

            Order? guestOrder = await _ordersRepository.GetByCartIdAsync(guestCartId);

            if (guestOrder != null)
            {
                Error error = EntityErrors<Order, int>.EntityInUse(guestOrder.Id, guestCartId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            Cart? guestCart = await _cartsRepository.GetByIdAsync(guestCartId);

            if (guestCart == null)
            {
                var error = EntityErrors<Cart, int>.NotFound(guestCartId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            order.CreateOrder(guestCart.Id, shippingDetail);
            order = await _ordersRepository.CreateOrderAsync(order);

            guestCart.CartStatus = CartStatus.Archive;
            await _cartsRepository.UpdateAsync(guestCart);

            AboutCartDomRes guestCartContent = guestCart.CheckCart();
            OrderResponse guestOrderContents = MapAboutCartToOrderRes(order, guestCartContent, shippingDetailsRes);

            await _emailService.SendOrderSummary(guestOrderContents);

            if (!string.IsNullOrWhiteSpace(method))
            {
                _guestSessionService.SetCartIdCookieAsExpired();
            }

            return EntityResult<OrderResponse>.Success(guestOrderContents);
        }

        public async Task<EntityResult<OrderResponse>> GetByIdAsync(int orderId)
        {
            if (orderId <= 0)
                return EntityResult<OrderResponse>.Failure(EntityErrors<Order, int>.WrongEntityId(orderId));

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                var error = EntityErrors<Order, int>.NotFound(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            AboutCartDomRes cartContent = order.Cart.CheckCart();
            ShippingDetailResponse shippingDetails = _mapper.Map<ShippingDetailResponse>(order.ShippingDetail);
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, cartContent, shippingDetails);

            return EntityResult<OrderResponse>.Success(orderResponse);
        }

        public async Task<EntityResult<OrderResponse>> DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
                return EntityResult<OrderResponse>.Failure(EntityErrors<Cart, int>.WrongEntityId(orderId));

            Order? order = await _ordersRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                var error = EntityErrors<Order, int>.NotFound(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            await _ordersRepository.DeleteOrderAsync(order);

            return EntityResult<OrderResponse>.Success();
        }

        public async Task<EntityResult<OrderResponse>> MarkOrderAsDeletedAsync(int orderId)
        {
            if (orderId <= 0)
                return EntityResult<OrderResponse>.Failure(EntityErrors<Cart, int>.WrongEntityId(orderId));

            bool isUserAuthenticated = _httpContextService.IsUserAuthenticated();
            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated == false)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<OrderResponse>.Failure(error);
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                var error = EntityErrors<Order, int>.NotFound(orderId);

                return EntityResult<OrderResponse>.Failure(error);
            }

            if (isUserAuthenticated)
            {
                string userId = _httpContextService.GetUserId();
                if (order?.Cart?.StoreUser?.Id == userId)
                {
                    order.MarkAsDeleted();

                    await _ordersRepository.UpdateAsync(order);

                    return EntityResult<OrderResponse>.Success();
                }
            }

            if (order.Cart.Id == receiveCartIdResult.Value)
            {
                order.MarkAsDeleted();

                await _ordersRepository.UpdateAsync(order);

                return EntityResult<OrderResponse>.Success();
            }

            var missingOrderOwnerError = EntityErrors<Order, int>.EntityDoesntBelongToYou(orderId);

            return EntityResult<OrderResponse>.Failure(missingOrderOwnerError);
        }

        public async Task<EntityResult<IEnumerable<OrdersResDto>>> GetOrdersAsync()
        {
            string userId = _httpContextService.GetUserId();

            IEnumerable<Order> userOrders = await _ordersRepository.GetByUserIdAsync(userId);
            IEnumerable<OrdersResDto> userOrdersRes = GetOrdersDetails(userOrders);

            return EntityResult<IEnumerable<OrdersResDto>>.Success(userOrdersRes);
        }

        private IEnumerable<OrdersResDto> GetOrdersDetails(IEnumerable<Order> userOrders)
        {
            return userOrders.Select(o => new OrdersResDto
            {
                Status = o.Status.ToString(),
                CartLineResponse = _mapper.Map<IEnumerable<CartLineResponse>>(o.Cart.CartLines),
                ShippingInfo = _mapper.Map<ShippingInfo>(o.ShippingDetail)
            });
        }

        private static OrderResponse MapAboutCartToOrderRes(Order order, AboutCartDomRes checkCart, ShippingDetailResponse shippingDetailsRes)
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