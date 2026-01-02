using System.Collections.Generic;
using System.Threading.Tasks;
using ChipAccess.Api.Repositories;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Api.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAuditLogService _audit;

        public EmployeeService(IEmployeeRepository repo, IAuditLogService audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public Task<List<Employee>> GetAllAsync() => _repo.GetAllActiveAsync();
        public Task<Employee?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<Employee?> GetByBamIdAsync(string bamId) => _repo.GetByBamIdAsync(bamId);

        public async Task<Employee> CreateAsync(Employee employee)
        {
            employee.IsActive = true;

            var created = await _repo.CreateAsync(employee);

            await _audit.LogAsync(
                employee.BamId,
                "Employee Created",
                "Employee",
                created.Id,
                $"Created employee {employee.FirstName} {employee.LastName}"
            );

            return created;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            bool ok = await _repo.UpdateAsync(employee);

            if (ok)
            {
                await _audit.LogAsync(
                    employee.BamId,
                    "Employee Updated",
                    "Employee",
                    employee.Id,
                    $"Updated fields for {employee.BamId}"
                );
            }

            return ok;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null)
                return false;

            emp.IsActive = false;

            bool ok = await _repo.UpdateAsync(emp);
            if (ok)
            {
                await _audit.LogAsync(
                    emp.BamId,
                    "Employee Deactivated",
                    "Employee",
                    emp.Id,
                    "User account disabled"
                );
            }

            return ok;
        }

        public Task<List<string>> SearchBamIdsAsync(string query)
        {
            return _repo.SearchBamIdsAsync(query);
        }

    }
}
