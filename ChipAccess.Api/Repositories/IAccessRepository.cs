using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Repositories
{
    public interface IAccessRepository
    {
        Task<List<AccessApprovalForm>> GetAllAsync();
        Task<AccessApprovalForm?> GetByIdAsync(int id);
        Task<List<AccessApprovalForm>> GetByBamIdAsync(string bamId);
        Task<List<AccessApprovalForm>> GetByStatusAsync(AccessStatus status);

        Task CreateAsync(AccessApprovalForm entity);
        Task<bool> UpdateAsync(AccessApprovalForm entity);
        Task<bool> DeleteAsync(int id);

        Task<int> ReplaceBamIdAsync(string oldBamId, string newBamId);
        Task<List<AccessApprovalForm>> GetManagedByAsync(string bamId);
    }
}