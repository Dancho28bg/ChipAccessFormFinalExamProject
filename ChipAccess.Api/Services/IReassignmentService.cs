using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Api.DTOs.Reassignment;

namespace ChipAccess.Api.Services
{
    public interface IReassignmentService
    {
        Task<ReassignmentQueue> CreateJobFromDtoAsync(CreateReassignmentDto dto, string requestedBy);

        Task<List<ReassignmentQueue>> GetPendingAsync();
        Task<List<ReassignmentQueue>> GetPendingForNewUserAsync(string bamId);

        Task<bool> AcceptAsync(int id, string currentUserBamId);

        Task<int> ProcessJobAsync(int id);
        Task<bool> RejectAsync(int id, string bamId, string reason);

    }
}
