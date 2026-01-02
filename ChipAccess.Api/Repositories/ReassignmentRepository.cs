using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipAccess.Api.Repositories
{
    public class ReassignmentRepository : IReassignmentRepository
    {
        private readonly AppDbContext _db;

        public ReassignmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateJobAsync(ReassignmentQueue job)
        {
            await _db.ReassignmentQueues.AddAsync(job);
            await _db.SaveChangesAsync();
        }

        public Task<ReassignmentQueue?> GetByIdAsync(int id)
            => _db.ReassignmentQueues.FindAsync(id).AsTask();

        public Task<List<ReassignmentQueue>> GetPendingAsync()
            => _db.ReassignmentQueues
                .AsNoTracking()
                .Where(r => !r.Processed)
                .ToListAsync();

        public Task<List<ReassignmentQueue>> GetPendingForNewUserAsync(string newBamId)
            => _db.ReassignmentQueues
                .AsNoTracking()
                .Where(r =>
                    r.NewBamId == newBamId &&
                    !r.AcceptedByNewUser &&
                    !r.Processed)
                .ToListAsync();

        public async Task<bool> AcceptAsync(int id)
        {
            var job = await _db.ReassignmentQueues.FindAsync(id);
            if (job == null || job.AcceptedByNewUser)
                return false;

            job.AcceptedByNewUser = true;
            job.AcceptedAt = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<int> ProcessJobAsync(int id)
        {
            var job = await _db.ReassignmentQueues.FindAsync(id);
            if (job == null || job.Processed || !job.AcceptedByNewUser)
                return 0;

            var accesses = await _db.AccessApprovalForms
                .Where(a => a.BamId == job.OldBamId)
                .ToListAsync();

            foreach (var a in accesses)
                a.BamId = job.NewBamId;

            job.Processed = true;
            job.ProcessedAt = DateTime.UtcNow;

            return await _db.SaveChangesAsync();
        }

        public async Task<bool> RejectAsync(int id, string reason)
        {
            var job = await _db.ReassignmentQueues.FindAsync(id);
            if (job == null || job.AcceptedByNewUser || job.RejectedByNewUser)
                return false;

            job.RejectedByNewUser = true;
            job.RejectReason = reason;
            job.RejectedAt = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }

    }
}
