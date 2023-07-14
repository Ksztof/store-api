using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using System.Collections.ObjectModel;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity
    {
        public int Id { get; set; }
        public Collection<CartLine> CartLine { get; set; }
    }
}
