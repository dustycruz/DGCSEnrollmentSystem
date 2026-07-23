// File: EnrollmentSystem.BLL/Services/Implementations/AuditService.cs
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditRepo;

    public AuditService(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task<AuditLogPageDto> GetLogsAsync(AuditLogFilterDto filter)
    {
        filter ??= new AuditLogFilterDto();
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 25;

        var (items, total) = await _auditRepo.GetPagedAsync(
            filter.Action,
            filter.EntityName,
            filter.Keyword,
            filter.FromDate,
            filter.ToDate,
            filter.Page,
            filter.PageSize);

        var rows = items.Select(a => new AuditLogRowDto
        {
            AuditLogId = a.AuditLogId,
            Timestamp = a.Timestamp,
            Action = a.Action ?? "-",
            EntityName = a.EntityName ?? "-",
            EntityId = a.EntityId ?? "-",
            Description = a.Description ?? "",
            Status = a.Status ?? "",
            Details = a.Details ?? "",
            Subject = a.Student is null
                ? "System"
                : $"{a.Student.LastName}, {a.Student.FirstName} ({a.Student.StudentNumber})"
        }).ToList();

        var totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)filter.PageSize);

        return new AuditLogPageDto
        {
            Rows = rows,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = total,
            TotalPages = totalPages,
            Actions = await _auditRepo.GetDistinctActionsAsync(),
            Entities = await _auditRepo.GetDistinctEntitiesAsync(),
            Filter = filter
        };
    }

    public async Task LogAsync(
        string action,
        string? entityName = null,
        string? entityId = null,
        string? description = null,
        string? status = null,
        string? details = null,
        int? studentId = null)
    {
        var entry = new AuditLog
        {
            Action = Trim(action, 100),
            EntityName = Trim(entityName, 150),
            EntityId = Trim(entityId, 100),
            Description = Trim(description, 500),
            Status = Trim(status, 50),
            Details = details,
            StudentId = studentId,
            Timestamp = DateTime.Now
        };

        await _auditRepo.AddAsync(entry);
        await _auditRepo.SaveAsync();
    }

    private static string? Trim(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= max ? value : value.Substring(0, max);
    }
}