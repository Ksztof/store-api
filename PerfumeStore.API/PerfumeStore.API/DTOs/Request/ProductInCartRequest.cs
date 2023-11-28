namespace PerfumeStore.API.DTOs.Request
{
    public class ProductInCartRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
