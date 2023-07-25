namespace PerfumeStore.Domain.Models
{
    public class CheckCartForm
    {
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
    }
}
