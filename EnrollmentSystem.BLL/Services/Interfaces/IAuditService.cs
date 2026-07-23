// File: EnrollmentSystem.BLL/Services/Interfaces/IAuditService.cs
using EnrollmentSystem.BLL.DTOs;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IAuditService
{
    Task<AuditLogPageDto> GetLogsAsync(AuditLogFilterDto filter);

    Task LogAsync(
        string action,
        string? entityName = null,
        string? entityId = null,
        string? description = null,
        string? status = null,
        string? details = null,
        int? studentId = null);
}