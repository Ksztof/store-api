using PerfumeStore.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class Product : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public CartLine CartLine { get; set; }
    }
}
