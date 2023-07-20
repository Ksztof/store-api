using PerfumeStore.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class CartLine : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
