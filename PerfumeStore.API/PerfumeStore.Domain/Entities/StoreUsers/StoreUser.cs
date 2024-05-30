using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.Orders;

namespace PerfumeStore.Domain.Entities.StoreUsers
{
    public class StoreUser : IdentityUser
    {
        public bool IsDeleteRequested { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
