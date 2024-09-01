using Store.Application.Orders.Dto.Response;
using Store.Domain.Shared.DTO.Response.Cart;

namespace Store.Application.Shared.DTO.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDomRes> AboutProductsInCart { get; set; }
        public ShippingDetailResponse ShippingDetil { get; set; }
    }
}