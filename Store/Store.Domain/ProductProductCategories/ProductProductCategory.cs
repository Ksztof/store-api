using Store.Domain.Abstractions;
using Store.Domain.ProductCategories;
using Store.Domain.Products;

namespace Store.Domain.ProductProductCategories;

public class ProductProductCategory : Entity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }

    public ProductProductCategory() { }
}