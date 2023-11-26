namespace PerfumeStore.Domain.DTOs.Request
{
    public class ProductInCartDomain
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}