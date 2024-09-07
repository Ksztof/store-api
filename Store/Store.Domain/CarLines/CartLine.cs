using Store.Domain.Abstractions;
using Store.Domain.Products;

namespace Store.Domain.CarLines;

public class CartLine : Entity
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public Product Product { get; set; }

    public CartLine(int productId)
    {
        ProductId = productId;
    }

    private CartLine() { }
}