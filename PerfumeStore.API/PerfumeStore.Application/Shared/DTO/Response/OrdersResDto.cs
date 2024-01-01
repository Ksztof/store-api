namespace PerfumeStore.Application.Shared.DTO.Response
{
    public class OrdersResDto
    {
        public string Status { get; set; }

        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
    }
}
