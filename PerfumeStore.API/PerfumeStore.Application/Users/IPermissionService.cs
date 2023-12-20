using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Users
{
    public interface IPermissionService
    {
        public void AssignVisitorRoleAsync(StoreUser storeUser);
        public void AssignAdminRoleAsync(StoreUser storeUser);

    }
}