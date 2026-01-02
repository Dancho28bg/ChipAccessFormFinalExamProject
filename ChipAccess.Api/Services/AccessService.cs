using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChipAccess.Api.DTOs.Access;
using ChipAccess.Api.Repositories;
using ChipAccess.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ChipAccess.Api.Services
{
    public class AccessService : IAccessService
    {
        private readonly IAccessRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IAuditLogService _audit;
        private readonly IAccessArchiveRepository _archiveRepo;
        private readonly ILogger<AccessService> _logger;

        public AccessService(
            IAccessRepository repo,
            IEmployeeRepository employeeRepo,
            IAuditLogService audit,
            IAccessArchiveRepository archiveRepo,
            ILogger<AccessService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _archiveRepo = archiveRepo ?? throw new ArgumentNullException(nameof(archiveRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<List<AccessApprovalForm>> GetAllAsync() => _repo.GetAllAsync();
        public Task<AccessApprovalForm?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<List<AccessApprovalForm>> GetByBamIdAsync(string bamId) => _repo.GetByBamIdAsync(bamId);
        public Task<List<AccessApprovalForm>> GetByStatusAsync(AccessStatus status) => _repo.GetByStatusAsync(status);
        public Task<List<AccessApprovalForm>> GetManagedByAsync(string bamId) => _repo.GetManagedByAsync(bamId);

        public async Task<List<AccessResponseDto>> GetAllDtoAsync()
        {
            var entities = await _repo.GetAllAsync();
            return await MapListToDtoAsync(entities);
        }

        public async Task<AccessResponseDto?> GetByIdDtoAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return await MapToDtoAsync(entity);
        }

        public async Task<List<AccessResponseDto>> GetByBamIdDtoAsync(string bamId)
        {
            var entities = await _repo.GetByBamIdAsync(bamId);
            return await MapListToDtoAsync(entities);
        }

        public async Task<List<AccessResponseDto>> GetByStatusDtoAsync(AccessStatus status)
        {
            var entities = await _repo.GetByStatusAsync(status);
            return await MapListToDtoAsync(entities);
        }

        public async Task<List<AccessResponseDto>> GetManagedByDtoAsync(string bamId)
        {
            var entities = await _repo.GetManagedByAsync(bamId);
            return await MapListToDtoAsync(entities);
        }

        public async Task<AccessApprovalForm> CreateFromDtoAsync(CreateAccessRequestDto dto, string requesterBamId)
        {
            var entity = new AccessApprovalForm
            {
                BamId = requesterBamId,
                Approver = dto.Approver,
                AccessNeeded = dto.AccessNeeded,
                Reason = dto.Reason,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = dto.ExpirationDate,
                ModifiedDate = null,
                RevokedDate = null,
                Status = AccessStatus.PendingManager
            };

            await _repo.CreateAsync(entity);

            await _audit.LogAsync(
                requesterBamId,
                "Created Access Request",
                "AccessApprovalForm",
                entity.Id,
                $"AccessNeeded={entity.AccessNeeded}"
            );

            return entity;
        }

        public async Task<UpdateResult> UpdateFromDtoAsync(
            int id,
            UpdateAccessRequestDto dto,
            string requesterBamId,
            string requesterRole)
        {
            var form = await _repo.GetByIdAsync(id);
            if (form == null)
                return UpdateResult.NotFound;

            bool isOwner = string.Equals(form.BamId, requesterBamId, StringComparison.OrdinalIgnoreCase);
            bool isPrivileged = requesterRole == "Admin" || requesterRole == "IT";

            if (!isOwner && !isPrivileged)
                return UpdateResult.Forbidden;

            if (form.Status != AccessStatus.PendingManager &&
                form.Status != AccessStatus.PendingAdmin)
            {
                return UpdateResult.InvalidState;
            }

            if (dto.ExpirationDate <= DateTime.UtcNow)
                return UpdateResult.InvalidState;

            form.Approver = dto.Approver;
            form.AccessNeeded = dto.AccessNeeded;
            form.Reason = dto.Reason;
            form.ExpirationDate = dto.ExpirationDate;

            form.ModifiedDate = DateTime.UtcNow;
            form.Status = AccessStatus.PendingManager;

            await _repo.UpdateAsync(form);

            await _audit.LogAsync(
                requesterBamId,
                "Updated Access Request",
                "AccessApprovalForm",
                id,
                $"Edited by {requesterRole}"
            );

            return UpdateResult.Success;
        }

        public async Task<bool> RejectAsync(int id, string requesterBamId, string requesterRole)
        {
            var form = await _repo.GetByIdAsync(id);
            if (form == null) return false;

            bool isManager = requesterRole == "Manager";
            bool isAdmin = requesterRole == "Admin" || requesterRole == "IT";

            if (isManager && form.Status != AccessStatus.PendingManager)
                return false;

            if (isAdmin && form.Status != AccessStatus.PendingAdmin)
                return false;

            form.Status = isManager
                ? AccessStatus.RejectedByManager
                : AccessStatus.RejectedByAdmin;

            form.RejectedBy = requesterBamId;
            form.ModifiedDate = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(form);
            if (!ok) return false;

            await _audit.LogAsync(
                requesterBamId,
                "Rejected Access Request",
                "AccessApprovalForm",
                id,
                form.Status.ToString()
            );

            return true;
        }

        public async Task<bool> RevokeAsync(int id)
        {
            var form = await _repo.GetByIdAsync(id);
            if (form == null) return false;

            form.Status = AccessStatus.Revoked;
            form.RevokedDate = DateTime.UtcNow;
            form.ModifiedDate = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(form);
            if (!ok) return false;

            await ArchiveFormAsync(form, AccessStatus.Revoked);

            await _audit.LogAsync(
                "SYSTEM",
                "Revoked Access",
                "AccessApprovalForm",
                id,
                "Archived after revocation"
            );

            return ok;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var form = await _repo.GetByIdAsync(id);
            if (form == null) return false;

            await ArchiveFormAsync(form, form.Status);

            var ok = await _repo.DeleteAsync(id);
            if (!ok) return false;

            await _audit.LogAsync(
                "SYSTEM",
                "Deleted Access Request",
                "AccessApprovalForm",
                id,
                "Archived then deleted"
            );

            return ok;
        }

        public async Task<int> ReplaceBamIdAsync(string oldBamId, string newBamId)
        {
            int count = await _repo.ReplaceBamIdAsync(oldBamId, newBamId);

            await _audit.LogAsync(
                "SYSTEM",
                "Reassigned BAMID",
                "AccessApprovalForm",
                null,
                $"Old={oldBamId}, New={newBamId}, Affected={count}"
            );

            return count;
        }

        public async Task<int> UpdateExpiredStatusesAsync()
        {
            var all = await _repo.GetAllAsync();
            int count = 0;

            foreach (var form in all)
            {
                if (form.Status == AccessStatus.Active &&
                    form.ExpirationDate.HasValue &&
                    form.ExpirationDate.Value < DateTime.UtcNow)
                {
                    form.Status = AccessStatus.Expired;
                    form.ModifiedDate = DateTime.UtcNow;
                    await _repo.UpdateAsync(form);

                    await ArchiveFormAsync(form, AccessStatus.Expired);

                    count++;
                }
            }

            if (count > 0)
            {
                await _audit.LogAsync(
                    "SYSTEM",
                    "Auto-Expired Access Records",
                    "AccessApprovalForm",
                    null,
                    $"Archived {count} expired entries"
                );
            }

            return count;
        }

        public async Task<int> UpdateExpiringSoonStatusesAsync()
        {
            var all = await _repo.GetAllAsync();
            int count = 0;

            foreach (var form in all)
            {
                if (form.Status == AccessStatus.Active &&
                    form.ExpirationDate.HasValue &&
                    (form.ExpirationDate.Value - DateTime.UtcNow).TotalDays <= 30)
                {
                    form.Status = AccessStatus.ExpiringSoon;
                    form.ModifiedDate = DateTime.UtcNow;
                    await _repo.UpdateAsync(form);
                    count++;
                }
            }

            return count;
        }

        public async Task<bool> ApproveAsync(int id, string approverRole, string approverBamId)
        {
            var form = await _repo.GetByIdAsync(id);
            if (form == null) return false;

            bool isManager = approverRole == "Manager";
            bool isAdmin = approverRole == "Admin" || approverRole == "IT";

            if (isManager)
            {
                if (form.Status != AccessStatus.PendingManager) return false;

                form.Status = AccessStatus.PendingAdmin;
                form.ModifiedDate = DateTime.UtcNow;
                await _repo.UpdateAsync(form);

                await _audit.LogAsync(
                    approverBamId,
                    "Manager Approved Request",
                    "AccessApprovalForm",
                    id,
                    "Moved to PendingAdmin"
                );

                return true;
            }

            if (isAdmin)
            {
                if (form.Status != AccessStatus.PendingAdmin) return false;

                form.Status = AccessStatus.Active;
                form.ModifiedDate = DateTime.UtcNow;
                await _repo.UpdateAsync(form);

                await _audit.LogAsync(
                    approverBamId,
                    "Admin Approved Request",
                    "AccessApprovalForm",
                    id,
                    "Request is now Active"
                );

                return true;
            }

            return false;
        }

        private async Task ArchiveFormAsync(AccessApprovalForm form, AccessStatus finalStatus)
        {
            try
            {
                var archived = new AccessApprovalArchive
                {
                    OriginalFormId = form.Id,
                    BamId = form.BamId,
                    Approver = form.Approver,
                    AccessNeeded = form.AccessNeeded,
                    Reason = form.Reason,
                    CreatedDate = form.CreatedDate,
                    ExpirationDate = form.ExpirationDate,
                    ModifiedDate = form.ModifiedDate,
                    RevokedDate = form.RevokedDate,
                    FinalStatus = finalStatus,
                    ArchivedAt = DateTime.UtcNow,
                    RejectedBy = form.RejectedBy
                };

                await _archiveRepo.ArchiveAsync(archived);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive access form {Id}", form.Id);
            }
        }

        private async Task<AccessResponseDto> MapToDtoAsync(AccessApprovalForm entity)
        {
            string employeeName = entity.BamId;
            if (!string.IsNullOrWhiteSpace(entity.BamId))
            {
                try
                {
                    var emp = await _employeeRepo.GetByBamIdAsync(entity.BamId);
                    if (emp != null)
                    {
                        employeeName = $"{emp.FirstName} {emp.LastName}".Trim();
                    }
                }
                catch
                {
                
                }
            }

            return new AccessResponseDto
            {
                Id = entity.Id,
                BamId = entity.BamId,
                EmployeeName = employeeName,
                Approver = entity.Approver,
                AccessNeeded = entity.AccessNeeded,
                Reason = entity.Reason,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                ExpirationDate = entity.ExpirationDate,
                RevokedDate = entity.RevokedDate,
                Status = entity.Status,

                RejectedBy = entity.RejectedBy,
            };

        }

        private async Task<List<AccessResponseDto>> MapListToDtoAsync(List<AccessApprovalForm> list)
        {
            var result = new List<AccessResponseDto>(list.Count);

            foreach (var e in list)
            {
                result.Add(await MapToDtoAsync(e));
            }

            return result;
        }
    }
}
