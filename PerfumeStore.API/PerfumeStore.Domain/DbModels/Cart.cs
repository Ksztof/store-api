using PerfumeStore.Domain.Interfaces;
using System.Collections.ObjectModel;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity
    {
        public int Id { get; set; }
        public Collection<CartLine> CartLines { get; set; }
    }
}
