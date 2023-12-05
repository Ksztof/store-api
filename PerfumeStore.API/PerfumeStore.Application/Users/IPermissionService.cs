using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Users
{
    public interface IPermissionService
    {
        public void AssignRoleAsync(StoreUser storeUser);
    }
}