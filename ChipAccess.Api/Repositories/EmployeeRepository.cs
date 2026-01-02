using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using ChipAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipAccess.Api.Repositories
{
    

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _db;

        public EmployeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<List<Employee>> GetAllActiveAsync() =>
            _db.Employees.Where(e => e.IsActive).ToListAsync();

        public Task<Employee?> GetByIdAsync(int id) =>
            _db.Employees.FirstOrDefaultAsync(e => e.Id == id);

        public Task<Employee?> GetByBamIdAsync(string bamId) =>
            _db.Employees.FirstOrDefaultAsync(e => e.BamId == bamId);

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            _db.Employees.Update(employee);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<List<string>> SearchBamIdsAsync(string query, int limit = 20)
        {
            return await _db.Employees
                .Where(e =>
                    e.IsActive &&
                    EF.Functions.Like(e.BamId, query + "%")
                )
                .OrderBy(e => e.BamId)
                .Select(e => e.BamId)
                .Take(limit)
                .ToListAsync();
        }

    }
}