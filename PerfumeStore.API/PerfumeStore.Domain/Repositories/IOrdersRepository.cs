using PerfumeStore.Domain.Entities.Orders;

namespace PerfumeStore.Domain.Repositories
{
    public interface IOrdersRepository
    {
        public Task<Order> CreateOrderAsync(Order order);

        public Task<Order?> GetByIdAsync(int orderId);

        public Task DeleteOrderAsync(Order order);

        public Task DeleteOrdersAsync(IEnumerable<Order> orders);

        public Task UpdateAsync(Order order);

        public Task<ShippingDet> GetShippingDetailsByUserIdAsync(string userId);

        public Task<ShippingDet> GetShippingDetailsByCartIdAsync(int cartId);

        public Task<Order?> GetByCartIdAsync(int cartId);

        public Task<IEnumerable<Order>> GetByUserIdAsync(string userId);

        public Task<int?> OrderAlreadyExists(int cartId);
    }
}