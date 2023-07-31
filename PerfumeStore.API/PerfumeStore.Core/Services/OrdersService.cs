using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
    internal class OrdersService : IOrdersService
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

        public async Task<AboutCartResponse> CreateOrderAsync()
        {
            int? GuestCartId = _guestSessionService.GetCartId();
            Cart? cart = await _cartsRepository.GetByIdAsync(GuestCartId.Value);
            if (cart == null)
            {
                throw new EntityNotFoundException<Cart, int>($"Entity of type: {typeof(Cart)} not found. Entity Id: {GuestCartId.Value}");
            }

            Order order = new Order();
            order.CreateOrder(cart.Id);
            order = await _ordersRepository.CreateOrderAsync(order);
            AboutCartResponse orderResponse = cart.CheckCart();

            return orderResponse;
        }
    }
}
