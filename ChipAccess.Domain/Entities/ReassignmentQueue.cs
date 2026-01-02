using System;

namespace ChipAccess.Domain.Entities
{
    public class ReassignmentQueue
    {
        public int Id { get; set; }

        public string OldBamId { get; set; }
        public string NewBamId { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }

        public bool AcceptedByNewUser { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public bool RejectedByNewUser { get; set; }
        public string? RejectReason { get; set; }
        public DateTime? RejectedAt { get; set; }

        public bool Processed { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}