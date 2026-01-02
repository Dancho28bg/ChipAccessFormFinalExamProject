using System;

namespace ChipAccess.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        
        public string PerformedBy { get; set; }
        public string Action { get; set; }
        public int? TargetId { get; set; }
        public string TargetType { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}