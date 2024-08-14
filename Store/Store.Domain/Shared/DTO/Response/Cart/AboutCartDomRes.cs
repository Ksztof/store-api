namespace Store.Domain.Shared.DTO.Response.Cart
{
    public class AboutCartDomRes
    {
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDomRes> AboutProductsInCart { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}