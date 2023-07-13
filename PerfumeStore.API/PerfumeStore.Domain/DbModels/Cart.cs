using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity
    {
        public int CartId { get; set; }
        //public Collection<CartLine> CartLine { get; set; }//CartId - fk, productId - fk, Quantity - int
        public Dictionary<int, CartProduct>? CartProducts { get; set; }
    }
}
