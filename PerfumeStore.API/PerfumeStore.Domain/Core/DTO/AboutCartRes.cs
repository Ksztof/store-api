namespace PerfumeStore.Domain.Core.DTO
{
    public class AboutCartRes
    {
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
    }
}