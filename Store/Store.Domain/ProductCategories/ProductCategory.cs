using Store.Domain.Abstractions;
using Store.Domain.ProductProductCategories;

namespace Store.Domain.ProductCategories
{
    public class ProductCategory : Entity
    {

        public string Name { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; } = new List<ProductProductCategory>();

        public ProductCategory() { }
    }
}