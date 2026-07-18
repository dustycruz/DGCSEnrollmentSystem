// File: EnrollmentSystem.DAL/Repositories/Implementations/EducationalLevelRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class EducationalLevelRepository : GenericRepository<EducationalLevel>, IEducationalLevelRepository
{
    public EducationalLevelRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<EducationalLevel>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.Name)
            .ToListAsync();

    public async Task<EducationalLevel?> GetByNameAsync(string name)
        => await _dbSet.FirstOrDefaultAsync(e => e.Name == name && !e.IsDeleted);
}