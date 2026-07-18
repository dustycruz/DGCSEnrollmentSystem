// File: EnrollmentSystem.DAL/Repositories/Implementations/EnrollmentRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        => await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted && e.StudentId == studentId)
            .Include(e => e.Section).ThenInclude(s => s!.SchoolYear)
            .Include(e => e.Section).ThenInclude(s => s!.GradeLevel)
            .ToListAsync();

    public async Task<IEnumerable<Enrollment>> GetBySectionAsync(int sectionId)
        => await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted && e.SectionId == sectionId)
            .Include(e => e.Student)
            .OrderBy(e => e.Student.LastName)
            .ToListAsync();

    public async Task<IEnumerable<Enrollment>> GetByStatusAsync(string status)
        => await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted && e.Status == status)
            .Include(e => e.Student)
            .Include(e => e.Section)
            .ToListAsync();

    public async Task<Enrollment?> GetFullAsync(int enrollmentId)
        => await _dbSet
            .Include(e => e.Student)
            .Include(e => e.Section).ThenInclude(s => s!.GradeLevel)
            .Include(e => e.StudentSchedules).ThenInclude(ss => ss.Schedule)
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId && !e.IsDeleted);

    public async Task<bool> IsStudentEnrolledAsync(int studentId, int sectionId)
        => await _dbSet.AnyAsync(e => !e.IsDeleted && e.StudentId == studentId && e.SectionId == sectionId);

    public async Task<int> CountBySectionAsync(int sectionId)
        => await _dbSet.CountAsync(e => !e.IsDeleted && e.SectionId == sectionId);
}