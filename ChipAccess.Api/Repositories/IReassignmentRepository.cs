using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Repositories
{
    public interface IReassignmentRepository
    {
        Task CreateJobAsync(ReassignmentQueue job);
        Task<ReassignmentQueue?> GetByIdAsync(int id);

        Task<List<ReassignmentQueue>> GetPendingAsync(); // admin
        Task<List<ReassignmentQueue>> GetPendingForNewUserAsync(string newBamId);

        Task<bool> AcceptAsync(int id);

        Task<int> ProcessJobAsync(int id);
        Task<bool> RejectAsync(int id, string reason);

    }
}
