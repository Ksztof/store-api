using Store.Application.Orders.Dto.Request;
using Store.Application.Orders.Dto.Response;
using Store.Domain.Abstractions;

namespace Store.Application.Orders;

public interface IOrdersService
{
    public Task<EntityResult<OrderResponseDto>> SubmitOrderAsync(string? method, CreateOrderDtoApp createOrderDtoApp);
    public Task<EntityResult<OrderResponseDto>> GetOrderByIdAsync(int orderId);
    public Task<EntityResult<OrderResponseDto>> DeleteOrderAsync(int orderId);
    public Task<EntityResult<OrderResponseDto>> MarkOrderAsDeletedAsync(int orderId);
    public Task<EntityResult<IEnumerable<OrdersResDto>>> GetOrdersAsync();
}