// File: EnrollmentSystem.BLL/DTOs/ReportDtos.cs
namespace EnrollmentSystem.BLL.DTOs;

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int PendingEnrollments { get; set; }
    public int TotalSections { get; set; }
    public int TotalTeachers { get; set; }
}

public class EnrollmentStatDto
{
    public string SectionName { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
    public int EnrolledCount { get; set; }
}

public class ClassListRowDto
{
    public int Number { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class ReportCardLineDto
{
    public string Subject { get; set; } = string.Empty;
    public decimal? Q1 { get; set; }
    public decimal? Q2 { get; set; }
    public decimal? Q3 { get; set; }
    public decimal? Q4 { get; set; }
    public decimal? Final { get; set; }
}

public class ReportCardDto
{
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<ReportCardLineDto> Lines { get; set; } = new();
    public decimal? GeneralAverage { get; set; }
}

public class MasterListRowDto
{
    public int Number { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}