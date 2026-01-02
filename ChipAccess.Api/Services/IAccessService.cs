using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Api.DTOs.Access;

namespace ChipAccess.Api.Services
{
    public interface IAccessService
    {

        Task<List<AccessApprovalForm>> GetAllAsync();
        Task<AccessApprovalForm?> GetByIdAsync(int id);
        Task<List<AccessApprovalForm>> GetByBamIdAsync(string bamId);
        Task<List<AccessApprovalForm>> GetByStatusAsync(AccessStatus status);
        Task<List<AccessApprovalForm>> GetManagedByAsync(string bamId);

        Task<List<AccessResponseDto>> GetAllDtoAsync();
        Task<AccessResponseDto?> GetByIdDtoAsync(int id);
        Task<List<AccessResponseDto>> GetByBamIdDtoAsync(string bamId);
        Task<List<AccessResponseDto>> GetByStatusDtoAsync(AccessStatus status);
        Task<List<AccessResponseDto>> GetManagedByDtoAsync(string bamId);

        Task<AccessApprovalForm> CreateFromDtoAsync(CreateAccessRequestDto dto, string requesterBamId);

        Task<UpdateResult> UpdateFromDtoAsync(
            int id,
            UpdateAccessRequestDto dto,
            string requesterBamId,
            string requesterRole);

        Task<bool> ApproveAsync(int id, string approverRole, string approverBamId);
        Task<bool> RejectAsync(int id, string requesterBamId, string requesterRole);
        Task<bool> RevokeAsync(int id);
        Task<bool> DeleteAsync(int id);

        Task<int> ReplaceBamIdAsync(string oldBamId, string newBamId);

        Task<int> UpdateExpiredStatusesAsync();
        Task<int> UpdateExpiringSoonStatusesAsync();
    }
}