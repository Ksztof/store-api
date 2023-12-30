using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Repositories.Generics;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Entities.CarLines
{
    public class CartLine : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public decimal Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}