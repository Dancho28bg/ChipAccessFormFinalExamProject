using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Api.DTOs.Reassignment
{
    public class CreateReassignmentDto
    {
        [Required]
        public string OldBamId { get; set; }

        [Required]
        public string NewBamId { get; set; }
    }
}