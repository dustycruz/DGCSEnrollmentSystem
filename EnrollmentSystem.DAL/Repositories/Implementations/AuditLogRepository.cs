// File: EnrollmentSystem.DAL/Repositories/Implementations/AuditLogRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<AuditLog> Items, int Total)> GetPagedAsync(
        string? action,
        string? entityName,
        string? keyword,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 25;

        IQueryable<AuditLog> query = _dbSet.AsNoTracking().Include(a => a.Student);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(a => a.Timestamp >= from);
        }

        if (toDate.HasValue)
        {
            var toExclusive = toDate.Value.Date.AddDays(1);
            query = query.Where(a => a.Timestamp < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(a =>
                (a.Description != null && EF.Functions.Like(a.Description, $"%{k}%")) ||
                (a.Details != null && EF.Functions.Like(a.Details, $"%{k}%")) ||
                (a.EntityName != null && EF.Functions.Like(a.EntityName, $"%{k}%")) ||
                (a.EntityId != null && EF.Functions.Like(a.EntityId, $"%{k}%")) ||
                (a.Action != null && EF.Functions.Like(a.Action, $"%{k}%")));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .ThenByDescending(a => a.AuditLogId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<string>> GetDistinctActionsAsync()
        => await _dbSet.AsNoTracking()
            .Where(a => a.Action != null && a.Action != "")
            .Select(a => a.Action!)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();

    public async Task<List<string>> GetDistinctEntitiesAsync()
        => await _dbSet.AsNoTracking()
            .Where(a => a.EntityName != null && a.EntityName != "")
            .Select(a => a.EntityName!)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
}