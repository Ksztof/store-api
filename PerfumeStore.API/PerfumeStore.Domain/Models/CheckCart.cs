namespace PerfumeStore.Domain.Models
{
    public class CheckCartDto
    {
        public int ProductId { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
