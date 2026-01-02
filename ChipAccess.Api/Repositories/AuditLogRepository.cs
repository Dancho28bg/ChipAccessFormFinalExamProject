using ChipAccess.Domain.Entities;
using ChipAccess.Infrastructure.Data;
using ChipAccess.Api.DTOs.AuditLog;
using ChipAccess.Api.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChipAccess.Api.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _db;

        public AuditLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task WriteAsync(AuditLog log)
        {
            await _db.AuditLogs.AddAsync(log);
            await _db.SaveChangesAsync();
        }

        public Task<List<AuditLog>> GetAllAsync()
            => _db.AuditLogs
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

        public async Task<PagedResult<AuditLog>> QueryAsync(AuditLogQueryDto filters)
        {
            var query = _db.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.PerformedBy))
                query = query.Where(x => x.PerformedBy.Contains(filters.PerformedBy));

            if (filters.From.HasValue)
                query = query.Where(x => x.Timestamp >= filters.From.Value);

            if (filters.To.HasValue)
                query = query.Where(x => x.Timestamp <= filters.To.Value);

            query = query.OrderByDescending(x => x.Timestamp);

            int total = await query.CountAsync();

            var items = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<AuditLog>
            {
                Items = items,
                TotalCount = total,
                Page = filters.Page,
                PageSize = filters.PageSize
            };
        }

        public async Task<List<AuditLog>> QueryAllAsync(AuditLogQueryDto filters)
        {
            var query = _db.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.PerformedBy))
                query = query.Where(x => x.PerformedBy.Contains(filters.PerformedBy));

            if (filters.From.HasValue)
                query = query.Where(x => x.Timestamp >= filters.From.Value);

            if (filters.To.HasValue)
                query = query.Where(x => x.Timestamp <= filters.To.Value);

            return await query
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
