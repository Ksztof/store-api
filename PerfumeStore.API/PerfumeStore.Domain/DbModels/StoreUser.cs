using Microsoft.AspNetCore.Identity;

namespace PerfumeStore.Domain.DbModels
{
    public class StoreUser : IdentityUser
    {

        public bool IsDeleteRequested { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
