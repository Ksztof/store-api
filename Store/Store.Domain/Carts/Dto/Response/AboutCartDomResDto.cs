namespace Store.Domain.Carts.Dto.Response;

public class AboutCartDomResDto
{
    public decimal TotalCartValue { get; set; }
    public IEnumerable<CheckCartDomRes> AboutProductsInCart { get; set; }
    public DateTime CreatedAt { get; set; }
}