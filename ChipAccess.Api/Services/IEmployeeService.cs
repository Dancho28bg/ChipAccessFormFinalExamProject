using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByBamIdAsync(string bamId);
        Task<Employee> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        
        Task<bool> DeactivateAsync(int id); 
        Task<List<string>> SearchBamIdsAsync(string query);

    }
}