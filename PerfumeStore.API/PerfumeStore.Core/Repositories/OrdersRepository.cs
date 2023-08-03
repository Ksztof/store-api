using Microsoft.EntityFrameworkCore;
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

        public async Task DeleteOrderAsync(Order order)
        {
            EntityEntry<Order> deleteOrder =  _shopDbContext.Orders.Remove(order);
            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            Order? order = await _shopDbContext.Orders
                .AsSingleQuery()
                .Include(c => c.Cart)
                .ThenInclude(cl => cl.CartLines)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(X => X.Id == orderId);

            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            EntityEntry<Order> orderUpdate = _shopDbContext.Orders.Update(order);
            await _shopDbContext.SaveChangesAsync();
        }
    }
}
