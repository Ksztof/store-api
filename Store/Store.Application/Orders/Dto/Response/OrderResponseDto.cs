using Store.Domain.Carts.Dto.Response;

namespace Store.Application.Orders.Dto.Response;

public class OrderResponseDto
{
    public int Id { get; set; }
    public decimal TotalCartValue { get; set; }
    public IEnumerable<CheckCartDomRes> AboutProductsInCart { get; set; }
    public ShippingDetailResponse ShippingDetil { get; set; }
}