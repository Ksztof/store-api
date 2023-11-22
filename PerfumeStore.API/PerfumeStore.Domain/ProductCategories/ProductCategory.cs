using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.ProductProductCategories;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.ProductCategories
{
    public class ProductCategory : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; } = new List<ProductProductCategory>();
    }
}