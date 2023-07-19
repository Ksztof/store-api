using PerfumeStore.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public ICollection<CartLine>? CartLines { get; set; }
    }
}
