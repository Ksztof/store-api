namespace PerfumeStore.Core.Models
{
    public class AboutCartResponse
    {
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
    }
}