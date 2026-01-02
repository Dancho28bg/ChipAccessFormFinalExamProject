using ChipAccess.Api.DTOs.Archive;
using ChipAccess.Api.Repositories;
using ChipAccess.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChipAccess.Api.Services
{
    public class AccessArchiveService : IAccessArchiveService
    {
        private readonly IAccessArchiveRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;

        public AccessArchiveService(
            IAccessArchiveRepository repo,
            IEmployeeRepository employeeRepo)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
        }

        public async Task<List<AccessArchiveResponseDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return await MapListAsync(entities);
        }

        public async Task<List<AccessArchiveResponseDto>> GetByBamIdAsync(string bamId)
        {
            var entities = await _repo.GetByBamIdAsync(bamId);
            return await MapListAsync(entities);
        }

        public async Task<List<AccessArchiveResponseDto>> GetManagedByAsync(string managerBamId)
        {
            var all = await _repo.GetAllAsync();

            var managed = all
                .Where(x => x.RejectedBy == managerBamId || x.Approver == managerBamId)
                .ToList();

            return await MapListAsync(managed);
        }

        private async Task<List<AccessArchiveResponseDto>> MapListAsync(
            List<AccessApprovalArchive> list)
        {
            var result = new List<AccessArchiveResponseDto>();

            foreach (var e in list)
            {
                string employeeName = e.BamId;
                var emp = await _employeeRepo.GetByBamIdAsync(e.BamId);
                if (emp != null)
                {
                    employeeName = $"{emp.FirstName} {emp.LastName}";
                }

                result.Add(new AccessArchiveResponseDto
                {
                    Id = e.Id,
                    OriginalFormId = e.OriginalFormId,
                    BamId = e.BamId,
                    Approver = e.Approver,
                    AccessNeeded = e.AccessNeeded,
                    Reason = e.Reason,
                    CreatedDate = e.CreatedDate,
                    ExpirationDate = e.ExpirationDate,
                    ModifiedDate = e.ModifiedDate,
                    RevokedDate = e.RevokedDate,
                    FinalStatus = e.FinalStatus,
                    ArchivedAt = e.ArchivedAt
                });
            }

            return result;
        }
    }
}
