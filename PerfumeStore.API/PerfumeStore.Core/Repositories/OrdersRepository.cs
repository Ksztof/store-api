using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        public readonly ShopDbContext _shopDbContext;
        public OrdersRepository(ShopDbContext shopDbContext)
        {
            _shopDbContext = shopDbContext;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            EntityEntry<Order> createOrder = await _shopDbContext.Orders.AddAsync(order);
            await _shopDbContext.SaveChangesAsync();

            return createOrder.Entity;
        }
    }
}
