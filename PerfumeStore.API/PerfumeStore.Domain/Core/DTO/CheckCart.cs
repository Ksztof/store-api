namespace PerfumeStore.Domain.Core.DTO
{
    public class CheckCartDto
    {
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}