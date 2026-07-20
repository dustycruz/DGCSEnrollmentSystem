// File: EnrollmentSystem.UI/ViewModels/AdminManageViewModels.cs
using System.ComponentModel.DataAnnotations;
using EnrollmentSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.ViewModels;

public class SectionCreateViewModel
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Select a school year.")] public int? SchoolYearId { get; set; }
    [Required(ErrorMessage = "Select a grade level.")] public int? GradeLevelId { get; set; }
    public int? CurriculumId { get; set; }

    public IEnumerable<SelectListItem> SchoolYears { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> GradeLevels { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Curricula { get; set; } = new List<SelectListItem>();
}

public class ScheduleCreateViewModel
{
    public int SectionId { get; set; }
    [Required] public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public string? Day { get; set; }
    [DataType(DataType.Time)] public TimeOnly? StartTime { get; set; }
    [DataType(DataType.Time)] public TimeOnly? EndTime { get; set; }
    public int? RoomId { get; set; }

    public IEnumerable<SelectListItem> Subjects { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Teachers { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Rooms { get; set; } = new List<SelectListItem>();
}

public class SectionDetailsViewModel
{
    public Section Section { get; set; } = new();
    public ScheduleCreateViewModel AddSchedule { get; set; } = new();
}

public class SubjectCreateViewModel
{
    public int SubjectId { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

public class AnnouncementCreateViewModel
{
    [Required] public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class EnrollmentManageViewModel
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public IEnumerable<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public IEnumerable<SelectListItem> Students { get; set; } = new List<SelectListItem>();
}