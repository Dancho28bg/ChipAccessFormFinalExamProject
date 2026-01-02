using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Api.DTOs.Archive;

namespace ChipAccess.Api.Services
{
    public interface IAccessArchiveService
    {
        Task<List<AccessArchiveResponseDto>> GetAllAsync();
        Task<List<AccessArchiveResponseDto>> GetByBamIdAsync(string bamId);
        Task<List<AccessArchiveResponseDto>> GetManagedByAsync(string managerBamId);
    }
}
