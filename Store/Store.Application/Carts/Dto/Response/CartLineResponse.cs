namespace Store.Application.Carts.Dto.Response;

public class CartLineResponse
{
    public int? productId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}