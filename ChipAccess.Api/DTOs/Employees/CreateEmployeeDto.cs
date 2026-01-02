using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Api.DTOs.Employees
{
    public class CreateEmployeeDto
    {
        [Required]
        public string BamId { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}