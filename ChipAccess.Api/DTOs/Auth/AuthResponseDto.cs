namespace ChipAccess.Api.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string BamId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}