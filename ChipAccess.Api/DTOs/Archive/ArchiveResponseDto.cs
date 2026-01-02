using System;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.DTOs.Archive
{
    public class AccessArchiveResponseDto
    {
        public int Id { get; set; }
        public int OriginalFormId { get; set; }
        public string BamId { get; set; }
        public string Approver { get; set; }
        public string AccessNeeded { get; set; }
        public string Reason { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? RevokedDate { get; set; }

        public AccessStatus FinalStatus { get; set; }
        public DateTime ArchivedAt { get; set; }
    }
}