using Store.Domain.StoreUsers;

namespace Store.Application.Users.Dto.Request;

public class RegisterUserDtoApp
{
    public StoreUser StoreUser { get; set; }
    public string Password { get; set; }
}
