// File: EnrollmentSystem.DAL/Repositories/Interfaces/IAuditLogRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<(IReadOnlyList<AuditLog> Items, int Total)> GetPagedAsync(
        string? action,
        string? entityName,
        string? keyword,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize);

    Task<List<string>> GetDistinctActionsAsync();
    Task<List<string>> GetDistinctEntitiesAsync();
}