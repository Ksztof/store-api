using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Orders;

namespace PerfumeStore.Domain.StoreUsers
{
    public class StoreUser : IdentityUser
    {
        public bool IsDeleteRequested { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public StoreUser() { }
    }
}
