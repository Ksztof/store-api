using Store.Application.Shared.DTO.Response;

namespace Store.Application.Orders.Dto.Response
{
    public class OrdersResDto
    {
        public string Status { get; set; }

        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
    }
}
