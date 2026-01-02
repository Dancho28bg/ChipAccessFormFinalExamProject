using System;
using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Api.DTOs.Access
{
    public class UpdateAccessRequestDto
    {
        [Required]
        public string Approver { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccessNeeded { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }
    }
}