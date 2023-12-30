using Microsoft.AspNetCore.Identity;
using PerfumeStore.Domain.Entities.Carts;

namespace PerfumeStore.Domain.Entities.StoreUsers
{
    public class StoreUser : IdentityUser
    {
        public bool IsDeleteRequested { get; set; }
        public Cart? Cart { get; set; }
    }
}
