using System.ComponentModel.DataAnnotations;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Domain.ProductProductCategories
{
    public class ProductProductCategory
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}