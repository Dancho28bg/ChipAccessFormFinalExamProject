using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Api.DTOs.AuditLog;
using ChipAccess.Api.DTOs.Common;

namespace ChipAccess.Api.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string performedBy, string action, string targetType, int? targetId, string details);
        Task<List<AuditLog>> GetAllAsync();
        Task<PagedResult<AuditLog>> QueryAsync(AuditLogQueryDto filters);
        Task<List<AuditLog>> QueryAllAsync(AuditLogQueryDto filters);
    }
}