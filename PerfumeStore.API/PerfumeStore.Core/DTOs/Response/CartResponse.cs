namespace PerfumeStore.Core.DTOs.Response
{
    public class CartResponse
    {
        public int Id { get; set; }
        public IEnumerable<CartLineResponse> CartLineDto { get; set; }
    }
}