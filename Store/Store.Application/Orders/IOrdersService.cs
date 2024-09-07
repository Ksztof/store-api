using Store.Application.Orders.Dto.Request;
using Store.Application.Orders.Dto.Response;
using Store.Domain.Abstractions;

namespace Store.Application.Orders;

public interface IOrdersService
{
    public Task<EntityResult<OrderResponse>> SubmitOrderAsync(string? method, CreateOrderDtoApp createOrderDtoApp);
    public Task<EntityResult<OrderResponse>> GetOrderByIdAsync(int orderId);
    public Task<EntityResult<OrderResponse>> DeleteOrderAsync(int orderId);
    public Task<EntityResult<OrderResponse>> MarkOrderAsDeletedAsync(int orderId);
    public Task<EntityResult<IEnumerable<OrdersResDto>>> GetOrdersAsync();
}