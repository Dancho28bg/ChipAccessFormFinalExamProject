using System.Threading.Tasks;
using ChipAccess.Api.Repositories;
using ChipAccess.Api.DTOs.AuditLog;
using ChipAccess.Api.DTOs.Common;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogService(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public async Task LogAsync(string performedBy, string action, string targetType, int? targetId, string details)
        {
            var log = new AuditLog
            {
                PerformedBy = performedBy,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _repo.WriteAsync(log);
        }

        public Task<List<AuditLog>> GetAllAsync() => _repo.GetAllAsync();

        public Task<PagedResult<AuditLog>> QueryAsync(AuditLogQueryDto filters)
            => _repo.QueryAsync(filters);
        
        public Task<List<AuditLog>> QueryAllAsync(AuditLogQueryDto filters)
            => _repo.QueryAllAsync(filters);

    }
}