// File: EnrollmentSystem.DAL/Repositories/Implementations/StudentParentRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class StudentParentRepository : GenericRepository<StudentParent>, IStudentParentRepository
{
    public StudentParentRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<StudentParent>> GetByStudentAsync(int studentId)
        => await _dbSet.AsNoTracking()
            .Where(p => !p.IsDeleted && p.StudentId == studentId)
            .ToListAsync();
}