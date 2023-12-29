using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.DTOs.Request
{
    public class RegisterUserDtoApp
    {
        public StoreUser StoreUser { get; set; }
        public string Password { get; set; }
    }
}
