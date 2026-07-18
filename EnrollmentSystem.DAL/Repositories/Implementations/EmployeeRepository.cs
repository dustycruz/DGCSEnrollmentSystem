// File: EnrollmentSystem.DAL/Repositories/Implementations/EmployeeRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Employee>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .ToListAsync();

    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        => await _dbSet.FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber && !e.IsDeleted);

    public async Task<IEnumerable<Employee>> SearchAsync(string term)
    {
        term = (term ?? string.Empty).Trim();
        return await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        (e.FirstName.Contains(term) ||
                         e.LastName.Contains(term) ||
                         (e.EmployeeNumber != null && e.EmployeeNumber.Contains(term))))
            .OrderBy(e => e.LastName)
            .ToListAsync();
    }
}