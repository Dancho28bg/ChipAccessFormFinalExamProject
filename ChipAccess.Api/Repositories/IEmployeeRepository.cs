using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllActiveAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByBamIdAsync(string bamId);
        Task<Employee> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<List<string>> SearchBamIdsAsync(string query, int limit = 20);

    }
}