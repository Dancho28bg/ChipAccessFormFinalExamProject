using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipAccess.Api.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly AppDbContext _db;

        public AccessRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AccessApprovalForm>> GetAllAsync()
        {
            return await _db.AccessApprovalForms
                .Where(a =>
                    a.Status == AccessStatus.PendingAdmin  ||
                    a.Status == AccessStatus.Active        ||
                    a.Status == AccessStatus.ExpiringSoon
                )
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AccessApprovalForm?> GetByIdAsync(int id)
            => await _db.AccessApprovalForms.FindAsync(id);

        public async Task<List<AccessApprovalForm>> GetByBamIdAsync(string bamId)
            => await _db.AccessApprovalForms
                .Where(a => a.BamId == bamId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<AccessApprovalForm>> GetByStatusAsync(AccessStatus status)
            => await _db.AccessApprovalForms
                .Where(a => a.Status == status)
                .AsNoTracking()
                .ToListAsync();
        
        public async Task<List<AccessApprovalForm>> GetManagedByAsync(string bamId)
        {
            return await _db.AccessApprovalForms
                .Where(a => a.Approver == bamId || a.RejectedBy == bamId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task CreateAsync(AccessApprovalForm entity)
        {
            _db.AccessApprovalForms.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(AccessApprovalForm entity)
        {
            _db.AccessApprovalForms.Update(entity);
            var saved = await _db.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AccessApprovalForms.FindAsync(id);
            if (entity == null) return false;
            _db.AccessApprovalForms.Remove(entity);
            var saved = await _db.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<int> ReplaceBamIdAsync(string oldBamId, string newBamId)
        {
            var items = await _db.AccessApprovalForms
                .Where(a => a.BamId == oldBamId)
                .ToListAsync();

            foreach (var item in items)
                item.BamId = newBamId;

            var updated = await _db.SaveChangesAsync();
            return updated;
        }
    }
}
