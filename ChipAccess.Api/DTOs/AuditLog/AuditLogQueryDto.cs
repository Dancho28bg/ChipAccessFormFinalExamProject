using System;

namespace ChipAccess.Api.DTOs.AuditLog
{
    public class AuditLogQueryDto
    {
        public string? PerformedBy { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
