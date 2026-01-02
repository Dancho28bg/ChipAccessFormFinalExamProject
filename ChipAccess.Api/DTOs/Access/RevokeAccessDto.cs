using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Api.DTOs.Access
{
    public class RevokeAccessDto
    {
        [Required]
        public string Reason { get; set; }
    }
}