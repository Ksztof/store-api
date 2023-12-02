using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Orders
{
    public interface IOrdersService
    {
        public Task<EntityResult<OrderResponse>> CreateOrderAsync();

        public Task<EntityResult<OrderResponse>> GetByIdAsync(int orderId);

        public Task<EntityResult<OrderResponse>> DeleteOrderAsync(int orderId);

        public Task<EntityResult<OrderResponse>> MarkOrderAsDeletedAsync(int orderId);
    }
}