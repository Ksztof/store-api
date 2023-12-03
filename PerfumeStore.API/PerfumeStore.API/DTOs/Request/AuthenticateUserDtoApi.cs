using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.API.DTOs.Request
{
    public class AuthenticateUserDtoApi
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
