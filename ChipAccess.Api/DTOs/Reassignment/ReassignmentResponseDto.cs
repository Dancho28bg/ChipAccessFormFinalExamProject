using System;

namespace ChipAccess.Api.DTOs.Reassignment
{
    public class ReassignmentResponseDto
    {
        public int Id { get; set; }
        public string OldBamId { get; set; }
        public string NewBamId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public bool Processed { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}