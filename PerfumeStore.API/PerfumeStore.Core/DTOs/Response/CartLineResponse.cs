namespace PerfumeStore.Core.DTOs.Response
{
    public class CartLineResponse
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}