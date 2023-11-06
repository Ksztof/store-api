using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Domain
{
  public class ApplicationDbContext : IdentityDbContext<StoreUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
  }
}
