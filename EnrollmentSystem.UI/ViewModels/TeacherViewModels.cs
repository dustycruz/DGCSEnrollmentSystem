// File: EnrollmentSystem.UI/ViewModels/TeacherViewModels.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class TeacherDashboardViewModel
{
    public Teacher? Teacher { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public IEnumerable<Schedule> Schedules { get; set; } = new List<Schedule>();
}

public class GradeEntryRow
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
}

public class GradeEncodeViewModel
{
    public int ScheduleId { get; set; }
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    public int? GradeLevelId { get; set; }
    public int TeacherId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Quarter { get; set; } = "Q1";
    public List<GradeEntryRow> Rows { get; set; } = new();
}