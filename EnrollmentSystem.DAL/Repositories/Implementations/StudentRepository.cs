// File: EnrollmentSystem.DAL/Repositories/Implementations/StudentRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Student>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .ToListAsync();

    public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
        => await _dbSet.FirstOrDefaultAsync(s => s.StudentNumber == studentNumber && !s.IsDeleted);

    public async Task<Student?> GetFullProfileAsync(int studentId)
        => await _dbSet
            .Include(s => s.StudentParents)
            .Include(s => s.StudentSiblings)
            .Include(s => s.Enrollments).ThenInclude(e => e.Section)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);

    public async Task<IEnumerable<Student>> SearchAsync(string term)
    {
        term = (term ?? string.Empty).Trim();
        return await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted &&
                        (s.FirstName.Contains(term) ||
                         s.LastName.Contains(term) ||
                         (s.StudentNumber != null && s.StudentNumber.Contains(term)) ||
                         (s.LearnerReferenceNumber != null && s.LearnerReferenceNumber.Contains(term))))
            .OrderBy(s => s.LastName)
            .ToListAsync();
    }

    public async Task<string> GenerateStudentNumberAsync()
    {
        var year = DateTime.Now.Year;
        var prefix = $"DGCS-{year}-";
        var lastNumber = await _dbSet
            .Where(s => s.StudentNumber != null && s.StudentNumber.StartsWith(prefix))
            .OrderByDescending(s => s.StudentNumber)
            .Select(s => s.StudentNumber)
            .FirstOrDefaultAsync();

        var next = 1;
        if (!string.IsNullOrEmpty(lastNumber))
        {
            var tail = lastNumber.Substring(prefix.Length);
            if (int.TryParse(tail, out var parsed))
                next = parsed + 1;
        }
        return $"{prefix}{next:D4}";
    }
}