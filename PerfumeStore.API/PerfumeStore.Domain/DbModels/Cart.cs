using PerfumeStore.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity
    {
        [Key]
        public int Id { get; set; }
        public Collection<CartLine>? CartLines { get; set; }
    }
}
