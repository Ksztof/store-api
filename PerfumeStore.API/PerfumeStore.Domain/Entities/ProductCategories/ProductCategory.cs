using PerfumeStore.Domain.Entities.ProductProductCategories;
using PerfumeStore.Domain.Repositories.Generics;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Entities.ProductCategories
{
    public class ProductCategory : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; } = new List<ProductProductCategory>();
    }
}