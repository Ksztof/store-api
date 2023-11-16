namespace PerfumeStore.Core.Models
{
    public class CheckCartDto
    {
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}