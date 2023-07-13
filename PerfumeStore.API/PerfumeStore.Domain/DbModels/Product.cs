using PerfumeStore.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class Product : IEntity
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
