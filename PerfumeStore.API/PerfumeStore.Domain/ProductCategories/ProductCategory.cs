using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.ProductProductCategories;

namespace PerfumeStore.Domain.ProductCategories
{
    public class ProductCategory : Entity
    {

        public string Name { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; } = new List<ProductProductCategory>();

        public ProductCategory() { }
    }
}