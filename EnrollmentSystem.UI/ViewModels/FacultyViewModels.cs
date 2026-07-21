// File: EnrollmentSystem.UI/ViewModels/FacultyViewModels.cs
using System.ComponentModel.DataAnnotations;
using EnrollmentSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.ViewModels;

public class TeacherCreateViewModel
{
    [Required] public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    [Required] public string LastName { get; set; } = string.Empty;
    [Display(Name = "Employee Number (leave blank to auto-generate)")]
    public string? EmployeeNumber { get; set; }
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string? EmailAddress { get; set; }
}

public class ScheduleManageViewModel
{
    public IEnumerable<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ScheduleCreateViewModel AddForm { get; set; } = new();
    public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
}