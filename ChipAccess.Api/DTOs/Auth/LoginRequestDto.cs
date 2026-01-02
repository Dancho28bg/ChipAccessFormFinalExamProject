using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string BamId { get; set; }
    }
}