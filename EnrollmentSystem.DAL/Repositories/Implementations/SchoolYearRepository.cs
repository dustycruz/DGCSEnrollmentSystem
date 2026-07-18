// File: EnrollmentSystem.DAL/Repositories/Implementations/SchoolYearRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class SchoolYearRepository : GenericRepository<SchoolYear>, ISchoolYearRepository
{
    public SchoolYearRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<SchoolYear>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.Name)
            .ToListAsync();

    public async Task<SchoolYear?> GetByNameAsync(string name)
        => await _dbSet.FirstOrDefaultAsync(y => y.Name == name && !y.IsDeleted);

    public async Task<SchoolYear?> GetLatestAsync()
        => await _dbSet.AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.SchoolYearId)
            .FirstOrDefaultAsync();
}