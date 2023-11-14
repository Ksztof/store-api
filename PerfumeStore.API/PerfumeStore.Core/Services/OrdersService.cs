using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
    public class OrdersService : IOrdersService
    {
        public readonly IOrdersRepository _ordersRepository;
        private readonly IGuestSessionService _guestSessionService;
        private readonly ICartsRepository _cartsRepository;
        private readonly IValidationService _validationService;

        public OrdersService(IOrdersRepository ordersRepository, IGuestSessionService guestSessionService, ICartsRepository cartsRepository, IValidationService validationService)
        {
            _ordersRepository = ordersRepository;
            _guestSessionService = guestSessionService;
            _cartsRepository = cartsRepository;
            _validationService = validationService;
        }

        public async Task<OrderResponse> CreateOrderAsync()
        {
            int? GuestCartId = _guestSessionService.GetCartId();
            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>(cart.Id);
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
            var idValidation = _validationService.ValidateEntityId(orderId);
            if (!idValidation.IsValid)
            {
                IEnumerable<string> errors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();
                throw new Exception($"Id is not correct. Details: {errors}");
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>(order.Id);
            }

            AboutCartResponse aboutCart = order.Cart.CheckCart();
            OrderResponse orderResponse = MapAboutCartToOrderRes(order, aboutCart);

            return orderResponse;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var idValidation = _validationService.ValidateEntityId(orderId);
            if (!idValidation.IsValid)
            {
                IEnumerable<string> errors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();
                throw new Exception($"Id is not correct. Details: {errors}");
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>(order.Id);
            }

            await _ordersRepository.DeleteOrderAsync(order);
        }

        public async Task MarkOrderAsDeletedAsync(int orderId)
        {
            var idValidation = _validationService.ValidateEntityId(orderId);
            if (!idValidation.IsValid)
            {
                IEnumerable<string> errors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();
                throw new Exception($"Id is not correct. Details: {errors}");
            }

            Order? order = await _ordersRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException<Order, int>(order.Id);
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