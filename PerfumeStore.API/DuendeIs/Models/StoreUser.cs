using Microsoft.AspNetCore.Identity;

namespace PerfumeStore.Domain.DbModels
{
  public class StoreUser : IdentityUser
  {
    public bool IsDeleteRequested { get; set; }
  }
}
