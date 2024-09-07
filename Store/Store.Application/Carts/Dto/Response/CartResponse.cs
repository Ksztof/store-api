namespace Store.Application.Carts.Dto.Response;

public class CartResponse
{
    public int? CartId { get; set; }
    public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
}