namespace PerfumeStore.Domain.Shared.DTO.Response.Cart
{
    public class CheckCartDomRes
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public decimal Quantity { get; set; }
    }
}