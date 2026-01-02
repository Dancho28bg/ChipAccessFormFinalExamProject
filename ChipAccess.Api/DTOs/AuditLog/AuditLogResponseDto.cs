using System;

namespace ChipAccess.Api.DTOs.Logs
{
    public class AuditLogResponseDto
    {
        public int Id { get; set; }
        public string PerformedBy { get; set; }
        public string Action { get; set; }
        public string TargetType { get; set; }
        public int? TargetId { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}