using Store.Domain.Abstractions;

namespace Store.Domain.Orders;

public interface IOrdersRepository
{
    public Task<Order> CreateOrderAsync(Order order);
    public Task<EntityResult<Order>> GetByIdAsync(int orderId);
    public Task DeleteOrderAsync(Order order);
    public Task DeleteOrdersAsync(IEnumerable<Order> orders);
    public Task UpdateAsync(Order order);
    public Task<ShippingDet> GetShippingDetailsByUserIdAsync(string userId);
    public Task<ShippingDet> GetShippingDetailsByCartIdAsync(int cartId);
    public Task<EntityResult<Order>> GetByCartIdAsync(int cartId);
    public Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    public Task<int?> OrderAlreadyExists(int cartId);
    public Task<int> GetNewestOrderIdByUserIdAsync(string userId);
    public Task<int> GetOrderIdByCartIdAsync(int cartId);
}