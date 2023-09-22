using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
  public interface IOrdersRepository
  {
    public Task<Order> CreateOrderAsync(Order order);

    public Task<Order?> GetByIdAsync(int orderId);

    public Task DeleteOrderAsync(Order order);

    public Task UpdateAsync(Order order);
  }
}