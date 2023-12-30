namespace PerfumeStore.Application.DTOs.Response
{
    public class OrdersResDto
    {
        public string Status { get; set; }

        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
    }
}
