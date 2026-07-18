// File: EnrollmentSystem.DAL/Repositories/Implementations/SubjectRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Subject>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public async Task<Subject?> GetByCodeAsync(string code)
        => await _dbSet.FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted);
}