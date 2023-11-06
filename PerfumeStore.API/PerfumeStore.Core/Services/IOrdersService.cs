using PerfumeStore.Core.DTOs.Response;

namespace PerfumeStore.Core.Services
{
  public interface IOrdersService
  {
    public Task<OrderResponse> CreateOrderAsync();

    public Task<OrderResponse> GetByIdAsync(int orderId);

    public Task DeleteOrderAsync(int orderId);

    public Task MarkOrderAsDeletedAsync(int orderId);
  }
}