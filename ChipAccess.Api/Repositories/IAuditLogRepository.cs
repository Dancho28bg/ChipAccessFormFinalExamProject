using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Api.DTOs.AuditLog;
using ChipAccess.Api.DTOs.Common;

namespace ChipAccess.Api.Repositories
{
    public interface IAuditLogRepository
    {
        Task WriteAsync(AuditLog log);
        Task<PagedResult<AuditLog>> QueryAsync(AuditLogQueryDto filters);
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> QueryAllAsync(AuditLogQueryDto filters);

    }
}