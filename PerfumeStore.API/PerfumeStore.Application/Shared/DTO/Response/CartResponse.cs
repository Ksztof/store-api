namespace PerfumeStore.Application.Shared.DTO.Response
{
    public class CartResponse
    {
        public int? CartId { get; set; }
        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
    }
}