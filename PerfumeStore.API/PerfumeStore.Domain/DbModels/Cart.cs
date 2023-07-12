using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DbModels
{
    public class Cart : IEntity
    {
        public int CartId { get; set; }
        public Guid? UserId { get; set; }
        //public Collection<CartLine> CartLine { get; set; }//CartId - fk, productId - fk, Quantity - int
        public Dictionary<int, CartProduct>? CartProducts { get; set; }
    }
}
