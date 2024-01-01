using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.API.Shared.DTO.Request.StoreUser
{
    public class RegisterUserDtoApi
    {
        public string Login { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
