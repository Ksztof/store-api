using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.Application.Shared.DTO.Request
{
    public class RegisterUserDtoApp
    {
        public StoreUser StoreUser { get; set; }
        public string Password { get; set; }
    }
}
