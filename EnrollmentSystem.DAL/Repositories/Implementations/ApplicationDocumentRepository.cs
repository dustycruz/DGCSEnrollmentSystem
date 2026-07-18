// File: EnrollmentSystem.DAL/Repositories/Implementations/ApplicationDocumentRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class ApplicationDocumentRepository : GenericRepository<ApplicationDocument>, IApplicationDocumentRepository
{
    public ApplicationDocumentRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<ApplicationDocument>> GetByApplicationAsync(int applicationId)
        => await _dbSet.AsNoTracking()
            .Where(d => d.ApplicationId == applicationId)
            .Include(d => d.Document)
            .ToListAsync();
}