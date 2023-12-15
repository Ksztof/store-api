using PerfumeStore.Domain.ShippingDetails;

namespace PerfumeStore.Domain.Orders
{
    public interface IOrdersRepository
    {
        public Task<Order> CreateOrderAsync(Order order);

        public Task<Order?> GetByIdAsync(int orderId);

        public Task DeleteOrderAsync(Order order);

        public Task UpdateAsync(Order order);

        public Task<ShippingDet> GetShippingDetailsByUserIdAsync(string userId);

        public Task<ShippingDet> GetShippingDetailsByCartIdAsync(int cartId);

        public Task<IEnumerable<Order>> GetByCartIdAsync(int cartId);

        public Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    }
}