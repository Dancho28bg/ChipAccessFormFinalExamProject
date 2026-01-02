using System;
using System.ComponentModel.DataAnnotations;

namespace ChipAccess.Domain.Entities
{
    public enum AccessStatus
    {
        PendingManager,
        PendingAdmin,

        Active,
        ExpiringSoon,
        Expired,

        Revoked,

        RejectedByManager,
        RejectedByAdmin
    }

    public class AccessApprovalForm
    {
        public int Id { get; set; }

        [Required]
        public string BamId { get; set; }  

        
        [Required]
        public string Approver { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccessNeeded { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; }
        
        public string? RejectedBy { get; set; }
        public string? RejectReason { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        public DateTime? RevokedDate { get; set; }

        public AccessStatus Status { get; set; } = AccessStatus.PendingManager;
    }
}