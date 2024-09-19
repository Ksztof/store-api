namespace Store.Application.Carts.Dto.Response;

public class CartResponseDto
{
    public int? CartId { get; set; }
    public IEnumerable<CartLineResponseDto> CartLineResponse { get; set; }
}