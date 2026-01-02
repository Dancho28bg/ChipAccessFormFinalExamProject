using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Repositories
{
    public interface IAccessArchiveRepository
    {
        Task ArchiveAsync(AccessApprovalArchive entry);
        Task<List<AccessApprovalArchive>> GetAllAsync();
        Task<List<AccessApprovalArchive>> GetByBamIdAsync(string bamId);
        Task<List<AccessApprovalArchive>> GetByStatusAsync(AccessStatus status);
    }
}