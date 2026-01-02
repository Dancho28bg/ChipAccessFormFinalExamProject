using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Api.DTOs.Reassignment;
using ChipAccess.Api.Repositories;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Services
{
    public class ReassignmentService : IReassignmentService
    {
        private readonly IReassignmentRepository _repo;
        private readonly IAuditLogService _audit;

        public ReassignmentService(
            IReassignmentRepository repo,
            IAuditLogService audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public async Task<ReassignmentQueue> CreateJobFromDtoAsync(CreateReassignmentDto dto, string requestedBy)
        {
            var job = new ReassignmentQueue
            {
                OldBamId = dto.OldBamId,
                NewBamId = dto.NewBamId,
                RequestedBy = requestedBy,
                RequestedAt = DateTime.UtcNow
            };

            await _repo.CreateJobAsync(job);

            await _audit.LogAsync(
                requestedBy,
                "Created Reassignment",
                "ReassignmentQueue",
                job.Id,
                $"Old={job.OldBamId}, New={job.NewBamId}"
            );

            return job;
        }

        public Task<List<ReassignmentQueue>> GetPendingAsync()
            => _repo.GetPendingAsync();

        public Task<List<ReassignmentQueue>> GetPendingForNewUserAsync(string bamId)
            => _repo.GetPendingForNewUserAsync(bamId);

        public async Task<bool> AcceptAsync(int id, string currentUserBamId)
        {
            var job = await _repo.GetByIdAsync(id);
            if (job == null)
                return false;

            if (job.NewBamId != currentUserBamId)
                throw new UnauthorizedAccessException();

            var accepted = await _repo.AcceptAsync(id);
            if (!accepted)
                return false;

            await _audit.LogAsync(
                currentUserBamId,
                "Accepted Reassignment",
                "ReassignmentQueue",
                id,
                $"Accepted access from {job.OldBamId}"
            );

            int moved = await _repo.ProcessJobAsync(id);

            await _audit.LogAsync(
                "SYSTEM",
                "Auto-Processed Reassignment",
                "ReassignmentQueue",
                id,
                $"Moved {moved} access records from {job.OldBamId} to {job.NewBamId}"
            );

            return true;
        }

        public async Task<int> ProcessJobAsync(int id)
        {
            var result = await _repo.ProcessJobAsync(id);

            if (result > 0)
            {
                await _audit.LogAsync(
                    "SYSTEM",
                    "Processed Reassignment",
                    "ReassignmentQueue",
                    id,
                    "Access records reassigned"
                );
            }

            return result;
        }

        public async Task<bool> RejectAsync(int id, string bamId, string reason)
        {
            var job = await _repo.GetByIdAsync(id);
            if (job == null)
                return false;

            if (job.NewBamId != bamId)
                throw new UnauthorizedAccessException();

            var ok = await _repo.RejectAsync(id, reason);

            if (ok)
            {
                await _audit.LogAsync(
                    bamId,
                    "Rejected Reassignment",
                    "ReassignmentQueue",
                    id,
                    $"Rejected reassignment from {job.OldBamId}. Reason: {reason}"
                );
            }

            return ok;
        }

    }
}
