using Store.Application.Shared.DTO.Request;
using Store.Application.Shared.DTO.Response;
using Store.Domain.Abstractions;

namespace Store.Application.Orders
{
    public interface IOrdersService
    {
        public Task<EntityResult<OrderResponse>> CreateOrderAsync(string? method, CreateOrderDtoApp createOrderDtoApp);

        public Task<EntityResult<OrderResponse>> GetByIdAsync(int orderId);

        public Task<EntityResult<OrderResponse>> DeleteOrderAsync(int orderId);

        public Task<EntityResult<OrderResponse>> MarkOrderAsDeletedAsync(int orderId);

        public Task<EntityResult<IEnumerable<OrdersResDto>>> GetOrdersAsync();
    }
}