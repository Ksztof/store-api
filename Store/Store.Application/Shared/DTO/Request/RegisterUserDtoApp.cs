using Store.Domain.StoreUsers;

namespace Store.Application.Shared.DTO.Request
{
    public class RegisterUserDtoApp
    {
        public StoreUser StoreUser { get; set; }
        public string Password { get; set; }
    }
}
