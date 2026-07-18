// File: EnrollmentSystem.DAL/Repositories/Implementations/TeacherRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Teacher>> GetAllWithEmployeeAsync()
        => await _dbSet.AsNoTracking()
            .Where(t => !t.IsDeleted)
            .Include(t => t.Employee)
            .OrderBy(t => t.Employee.LastName)
            .ToListAsync();

    public async Task<Teacher?> GetWithEmployeeAsync(int teacherId)
        => await _dbSet
            .Include(t => t.Employee)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId && !t.IsDeleted);

    public async Task<Teacher?> GetByEmployeeIdAsync(int employeeId)
        => await _dbSet
            .Include(t => t.Employee)
            .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && !t.IsDeleted);
}