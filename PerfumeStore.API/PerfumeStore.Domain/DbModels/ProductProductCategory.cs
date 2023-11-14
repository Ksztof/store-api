using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
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