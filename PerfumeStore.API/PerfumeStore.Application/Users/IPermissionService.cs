using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Users
{
    public interface IPermissionService
    {
        public Task AssignVisitorRoleAsync(StoreUser storeUser);

        public Task AssignAdminRoleAsync(StoreUser storeUser);
    }
}