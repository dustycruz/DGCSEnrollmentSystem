// File: EnrollmentSystem.DAL/Repositories/Implementations/CurriculumRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
{
    public CurriculumRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Curriculum>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Include(c => c.EducationalLevel)
            .OrderBy(c => c.Code)
            .ToListAsync();

    public async Task<Curriculum?> GetWithSubjectsAsync(int curriculumId)
        => await _dbSet
            .Include(c => c.EducationalLevel)
            .Include(c => c.CurriculumSubjects).ThenInclude(cs => cs.Subject)
            .Include(c => c.CurriculumSubjects).ThenInclude(cs => cs.GradeLevel)
            .FirstOrDefaultAsync(c => c.CurriculumId == curriculumId && !c.IsDeleted);

    public async Task<IEnumerable<Curriculum>> GetByEducationalLevelAsync(int educationalLevelId)
        => await _dbSet.AsNoTracking()
            .Where(c => !c.IsDeleted && c.EducationalLevelId == educationalLevelId)
            .ToListAsync();
}