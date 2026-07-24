// File: EnrollmentSystem.UI/ViewModels/TeacherViewModels.cs
using EnrollmentSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.ViewModels;

public class TeacherAnnouncementsViewModel
{
    public List<Announcement> Posts { get; set; } = new();
    public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
    public Dictionary<int, string> SectionNames { get; set; } = new();
}

public class TeacherDashboardViewModel
{
    public Teacher? Teacher { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public IEnumerable<Schedule> Schedules { get; set; } = new List<Schedule>();

    public string Today { get; set; } = string.Empty;
    public List<TodayClassItem> TodayClasses { get; set; } = new();
    public int ClassCount { get; set; }
    public string? AdvisoryClassName { get; set; }
    public int AdviseeCount { get; set; }
    public int PendingGradeQuarters { get; set; }
}

public class TodayClassItem
{
    public int ScheduleId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public TimeOnly? Start { get; set; }
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