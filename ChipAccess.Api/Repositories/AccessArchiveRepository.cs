using ChipAccess.Domain.Entities;
using ChipAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChipAccess.Api.Repositories
{
    public class AccessArchiveRepository : IAccessArchiveRepository
    {
        private readonly AppDbContext _db;

        public AccessArchiveRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task ArchiveAsync(AccessApprovalArchive entry)
        {
            await _db.AccessApprovalArchives.AddAsync(entry);
            await _db.SaveChangesAsync();
        }

        public Task<List<AccessApprovalArchive>> GetAllAsync()
            => _db.AccessApprovalArchives
                .OrderByDescending(x => x.ArchivedAt)
                .ToListAsync();

        public Task<List<AccessApprovalArchive>> GetByBamIdAsync(string bamId)
            => _db.AccessApprovalArchives
                .Where(x => x.BamId == bamId)
                .OrderByDescending(x => x.ArchivedAt)
                .ToListAsync();

        public Task<List<AccessApprovalArchive>> GetByStatusAsync(AccessStatus status)
            => _db.AccessApprovalArchives
                .Where(x => x.FinalStatus == status)
                .OrderByDescending(x => x.ArchivedAt)
                .ToListAsync();
    }
}