// File: EnrollmentSystem.BLL/DTOs/DashboardChartsDto.cs
namespace EnrollmentSystem.BLL.DTOs;

public class DashboardChartsDto
{
    public List<string> GradeLabels { get; set; } = new();
    public List<int> GradeEnrollment { get; set; } = new();

    public List<string> AppStatusLabels { get; set; } = new();
    public List<int> AppStatusCounts { get; set; } = new();

    public List<string> GradeDistLabels { get; set; } = new();
    public List<int> GradeDistCounts { get; set; } = new();

    public List<string> PaymentLabels { get; set; } = new();
    public List<int> PaymentCounts { get; set; } = new();
}