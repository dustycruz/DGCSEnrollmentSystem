// File: EnrollmentSystem.UI/ViewModels/StudentViewModels.cs
using System.ComponentModel.DataAnnotations;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class StudentDashboardViewModel
{
    public Student? Student { get; set; }
    public IEnumerable<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public IEnumerable<Announcement> Announcements { get; set; } = new List<Announcement>();
}

public class StudentProfileViewModel
{
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;

    [Required] public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    [Required] public string LastName { get; set; } = string.Empty;

    public string? Address { get; set; }
    public string? MobileNumber { get; set; }
    [EmailAddress] public string? EmailAddress { get; set; }
    public string? HomeNumber { get; set; }

    public string? GuardianName { get; set; }
    public string? GuardianRelationship { get; set; }
    [EmailAddress] public string? GuardianEmailAddress { get; set; }
}