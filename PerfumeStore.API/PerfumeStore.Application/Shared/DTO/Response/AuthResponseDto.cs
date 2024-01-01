namespace PerfumeStore.Application.Shared.DTO.Response
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}
