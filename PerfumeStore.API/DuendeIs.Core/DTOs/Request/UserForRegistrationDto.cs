using System.ComponentModel.DataAnnotations;

namespace DuendeIs.Core.DTOs.Request
{
  public class UserForRegistrationDto
  {
    [Required(ErrorMessage = "Chose your login.")]
    public string UserName { get; set; }

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
