using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.API.Shared.DTO.Request.StoreUser
{
    public class AuthenticateUserDtoApi
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
