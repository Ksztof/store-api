using AutoMapper;
using Store.Application.Carts.Dto.Response;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Email;
using Store.Application.Contracts.Guest;
using Store.Application.Orders.Dto.Request;
using Store.Application.Orders.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Carts;
using Store.Domain.Carts.Dto.Response;
using Store.Domain.Orders;
using Store.Domain.Orders.Dto.Request;
using Store.Domain.Shared.Errors;
using Store.Domain.StoreUsers.Errors;

namespace Store.Application.Orders
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly ICartsRepository _cartsRepository;
        private readonly IHttpContextService _contextService;
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
            _contextService = httpContextService;
            _mapper = mapper;
            _guestSessionService = guestSessionService;
            _emailService = emailService;
        }

        public async Task<EntityResult<OrderResponse>> CreateOrderAsync(string? method, CreateOrderDtoApp createOrderDtoApp)
        {
            Result isUserAuthenticated = _contextService.IsUserAuthenticated();

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
            {
                Error error = UserErrors.CantAuthenticateByCartIdOrUserCookie;

                return EntityResult<OrderResponse>.Failure(error);
            }

            ShippingDet shippingDetail = new ShippingDet();
            CreateOrderDtoDom createOrderDtoDom = _mapper.Map<CreateOrderDtoDom>(createOrderDtoApp);
            shippingDetail.CreateShippingDetail(createOrderDtoDom);
            ShippingDetailResponse shippingDetailsRes = _mapper.Map<ShippingDetailResponse>(shippingDetail);

            Order order = new Order();

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<OrderResponse>.Failure(result.Error);
                }

                string userId = result.Value;

                EntityResult<Cart> getUserCart = await _cartsRepository.GetByUserIdAsync(userId);
                if (getUserCart.IsFailure)
                {
                    return EntityResult<OrderResponse>.Failure(getUserCart.Error);
                }

                Cart userCart = getUserCart.Entity;

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

            EntityResult<Cart> getGuestCart = await _cartsRepository.GetByIdAsync(guestCartId);
            if (getGuestCart.IsFailure)
            {
                return EntityResult<OrderResponse>.Failure(getGuestCart.Error);
            }

            Cart guestCart = getGuestCart.Entity;

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

            Result isUserAuthenticated = _contextService.IsUserAuthenticated();
            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure && isUserAuthenticated.IsFailure)
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

            if (isUserAuthenticated.IsSuccess)
            {
                Result<string> result = _contextService.GetUserId();
                if (result.IsFailure)
                {
                    return EntityResult<OrderResponse>.Failure(result.Error);
                }

                if (order?.Cart?.StoreUser?.Id == result.Value)
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
            Result<string> result = _contextService.GetUserId();
            if (result.IsFailure)
            {
                return EntityResult<IEnumerable<OrdersResDto>>.Failure(result.Error);
            }

            IEnumerable<Order> userOrders = await _ordersRepository.GetByUserIdAsync(result.Value);
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