using Store.Domain.StoreUsers;

namespace Store.Application.Users
{
    public interface IPermissionService
    {
        public Task AssignVisitorRoleAsync(StoreUser storeUser);

        public Task AssignAdminRoleAsync(StoreUser storeUser);
    }
}