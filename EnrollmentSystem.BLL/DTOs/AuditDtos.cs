// File: EnrollmentSystem.BLL/DTOs/AuditDtos.cs
namespace EnrollmentSystem.BLL.DTOs;

public class AuditLogFilterDto
{
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public class AuditLogRowDto
{
    public int AuditLogId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Subject { get; set; } = "System";
}

public class AuditLogPageDto
{
    public List<AuditLogRowDto> Rows { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
    public List<string> Actions { get; set; } = new();
    public List<string> Entities { get; set; } = new();
    public AuditLogFilterDto Filter { get; set; } = new();
}