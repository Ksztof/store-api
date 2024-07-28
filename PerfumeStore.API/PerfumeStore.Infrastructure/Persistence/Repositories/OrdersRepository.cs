using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PerfumeStore.Domain.Orders;

namespace PerfumeStore.Infrastructure.Persistence.Repositories
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
            EntityEntry<Order> deleteOrder = _shopDbContext.Orders.Remove(order);

            await _shopDbContext.SaveChangesAsync();
        }

        public async Task DeleteOrdersAsync(IEnumerable<Order> orders)
        {
            _shopDbContext.Orders.RemoveRange(orders);

            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            Order? order = await _shopDbContext.Orders
                .AsSingleQuery()
                .Include(c => c.Cart)
                .ThenInclude(cl => cl.CartLines)
                .ThenInclude(p => p.Product)
                .Include(sd => sd.ShippingDetail)
                .FirstOrDefaultAsync(X => X.Id == orderId);

            return order;
        }

        public async Task<ShippingDet> GetShippingDetailsByUserIdAsync(string userId)
        {
            Order order = await _shopDbContext.Orders
                .Include(x => x.ShippingDetail)
                .FirstOrDefaultAsync(x => x.Cart.StoreUserId == userId);

            ShippingDet shippingDetails = order.ShippingDetail;

            return shippingDetails;
        }
        public async Task<ShippingDet> GetShippingDetailsByCartIdAsync(int cartId)
        {
            Order order = await _shopDbContext.Orders
                .Include(x => x.ShippingDetail)
                .FirstOrDefaultAsync(x => x.CartId == cartId);

            ShippingDet shippingDetails = order.ShippingDetail;

            return shippingDetails;
        }

        /*public async Task<IEnumerable<Order>> GetByCartIdAsync(int cartId)
        {
            IEnumerable<Order> orders = await _shopDbContext.Orders
                .Include(x => x.ShippingDetail)
                .Where(x => x.CartId == cartId).ToListAsync();

            return orders;
        }*/

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            IEnumerable<Order> orders = await _shopDbContext.Orders
                .Include(o => o.Cart).ThenInclude(c => c.CartLines).ThenInclude(cl => cl.Product)
                .Include(o => o.ShippingDetail)
                .Where(o => o.StoreUserId == userId).ToListAsync();

            return orders;
        }

        public async Task UpdateAsync(Order order)
        {
            EntityEntry<Order> orderUpdate = _shopDbContext.Orders.Update(order);
            await _shopDbContext.SaveChangesAsync();
        }

        public async Task<int?> OrderAlreadyExists(int cartId)
        {
            Order? order = await _shopDbContext.Orders.FirstOrDefaultAsync(o => o.CartId == cartId);

            if (order != null)
                return order.Id;

            return null;
        }

        public async Task<Order?> GetByCartIdAsync(int cartId)
        {
            Order? order = await _shopDbContext.Orders.FirstOrDefaultAsync(o => o.CartId == cartId);

            if (order != null)
                return order;

            return null;
        }

        public async Task<int> GetNewestOrderIdByUserIdAsync(string userId)
        {
            int orderId = await _shopDbContext.Orders
                .Where(o => o.StoreUserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => o.Id)
                .FirstOrDefaultAsync();

            return orderId;
        }

        public async Task<int> GetOrderIdByCartIdAsync(int cartId)
        {
            int orderId = await _shopDbContext.Orders
                .Where(o => o.CartId == cartId)
                .Select(o => o.Id)
                .FirstOrDefaultAsync();

            return orderId;
        }
    }
}