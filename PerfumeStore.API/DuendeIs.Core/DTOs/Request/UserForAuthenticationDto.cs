using System.ComponentModel.DataAnnotations;

namespace DuendeIs.Core.DTOs.Request
{
  public class UserForAuthenticationDto
  {
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
  }
}
