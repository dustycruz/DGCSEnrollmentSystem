// File: EnrollmentSystem.DAL/Repositories/Interfaces/IApplicationRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IApplicationRepository : IGenericRepository<Application>
{
    Task<IEnumerable<Application>> GetAllActiveAsync();
    Task<IEnumerable<Application>> GetByStatusAsync(string status);
    Task<IEnumerable<Application>> GetPendingAsync();
    Task<Application?> GetFullApplicationAsync(int applicationId);
    Task<IEnumerable<Application>> GetByCreatedByAsync(string createdBy);
    Task<int> CountByStatusAsync(string status);
}