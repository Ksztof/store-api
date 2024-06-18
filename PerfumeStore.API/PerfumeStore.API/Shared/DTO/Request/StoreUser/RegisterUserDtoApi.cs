using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.API.Shared.DTO.Request.StoreUser
{
    public class RegisterUserDtoApi
    {
        public string Login { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
