using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Domain.ProductProductCategories
{
    public class ProductProductCategory : Entity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        public ProductProductCategory() { }
    }
}