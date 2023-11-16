using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.Carts;

namespace PerfumeStore.Domain.StoreUsers
{
    public class StoreUser : IdentityUser
    {

        public bool IsDeleteRequested { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
