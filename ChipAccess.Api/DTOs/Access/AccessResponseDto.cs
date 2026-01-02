using System;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.DTOs.Access
{
    public class AccessResponseDto
    {
        public int Id { get; set; }
        public string BamId { get; set; }
        public string EmployeeName { get; set; } 

        public string Approver { get; set; }
        public string AccessNeeded { get; set; }
        public string Reason { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? RevokedDate { get; set; }

        public AccessStatus Status { get; set; }

        public string? RejectedBy { get; set; }
    }
}