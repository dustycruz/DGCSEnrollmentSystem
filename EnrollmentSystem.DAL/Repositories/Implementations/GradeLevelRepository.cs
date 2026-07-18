// File: EnrollmentSystem.DAL/Repositories/Implementations/GradeLevelRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class GradeLevelRepository : GenericRepository<GradeLevel>, IGradeLevelRepository
{
    public GradeLevelRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<GradeLevel>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted)
            .Include(g => g.EducationalLevel)
            .OrderBy(g => g.GradeLevelId)
            .ToListAsync();

    public async Task<IEnumerable<GradeLevel>> GetByEducationalLevelAsync(int educationalLevelId)
        => await _dbSet.AsNoTracking()
            .Where(g => !g.IsDeleted && g.EducationalLevelId == educationalLevelId)
            .ToListAsync();
}