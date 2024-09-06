namespace Store.Application.Products.Dto.Request;

public class UpdateProductDtoApp
{
    public int productId { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductManufacturer { get; set; }
    public ICollection<int>? ProductCategoriesIds { get; set; }
}
