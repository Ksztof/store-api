namespace DuendeIs.Core.DTOs.Response
{
  public class RegistrationResponseDto
  {
    public bool IsSuccessfulRegistration { get; set; }
    public IEnumerable<string> Errors { get; set; }
    public string? Message { get; set; }
  }
}
