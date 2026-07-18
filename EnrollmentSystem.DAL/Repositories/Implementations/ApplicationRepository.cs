// File: EnrollmentSystem.DAL/Repositories/Implementations/ApplicationRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
{
    public ApplicationRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Application>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted)
            .Include(a => a.GradeLevel)
            .Include(a => a.SchoolYear)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Application>> GetByStatusAsync(string status)
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted && a.Status == status)
            .Include(a => a.GradeLevel)
            .Include(a => a.SchoolYear)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();

    public async Task<IEnumerable<Application>> GetPendingAsync()
        => await GetByStatusAsync("Pending");

    public async Task<Application?> GetFullApplicationAsync(int applicationId)
        => await _dbSet
            .Include(a => a.GradeLevel)
            .Include(a => a.SchoolYear)
            .Include(a => a.ApplicationParents)
            .Include(a => a.ApplicationSiblings)
            .Include(a => a.ApplicantAcademicRecords)
            .Include(a => a.ApplicationDocuments).ThenInclude(d => d.Document)
            .Include(a => a.ProofOfPayments)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && !a.IsDeleted);

    public async Task<IEnumerable<Application>> GetByCreatedByAsync(string createdBy)
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted && a.CreatedBy == createdBy)
            .Include(a => a.GradeLevel)
            .Include(a => a.SchoolYear)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();

    public async Task<int> CountByStatusAsync(string status)
        => await _dbSet.CountAsync(a => !a.IsDeleted && a.Status == status);
}