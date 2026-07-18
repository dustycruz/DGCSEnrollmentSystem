// File: EnrollmentSystem.DAL/Repositories/Implementations/GradeRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class GradeRepository : GenericRepository<Grade>, IGradeRepository
{
    public GradeRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Grade>> GetByStudentAsync(int studentId)
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted && g.StudentId == studentId)
            .Include(g => g.Subject)
            .Include(g => g.GradeLevel)
            .OrderBy(g => g.Subject.Name).ThenBy(g => g.Quarter)
            .ToListAsync();

    public async Task<IEnumerable<Grade>> GetByStudentAndQuarterAsync(int studentId, string quarter)
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted && g.StudentId == studentId && g.Quarter == quarter)
            .Include(g => g.Subject)
            .ToListAsync();

    public async Task<IEnumerable<Grade>> GetByTeacherAsync(int teacherId)
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted && g.TeacherId == teacherId)
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .ToListAsync();

    public async Task<IEnumerable<Grade>> GetBySubjectAsync(int subjectId)
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted && g.SubjectId == subjectId)
            .Include(g => g.Student)
            .ToListAsync();

    public async Task<Grade?> GetExistingAsync(int studentId, int subjectId, string quarter)
        => await _dbSet.FirstOrDefaultAsync(g =>
            !g.IsDeleted && g.StudentId == studentId && g.SubjectId == subjectId && g.Quarter == quarter);
}