using Microsoft.AspNetCore.Identity;

namespace DuendeIs.Core.Models
{
  public class StoreUser : IdentityUser
  {
    public bool IsDeleteRequested { get; set; }
  }
}
