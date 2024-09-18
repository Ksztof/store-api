using Store.Application.Carts.Dto.Response;

namespace Store.Application.Orders.Dto.Response;

public class OrdersResDto
{
    public string Status { get; set; }
    public IEnumerable<CartLineResponseDto> CartLineResponse { get; set; }
    public ShippingInfo ShippingInfo { get; set; }
}
