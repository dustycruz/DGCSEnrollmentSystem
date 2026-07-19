// File: EnrollmentSystem.UI/ViewModels/AdmissionFormViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.ViewModels;

public class AdmissionFormViewModel
{
    public int? ApplicationId { get; set; }

    // Personal
    [Required] public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    [Required] public string LastName { get; set; } = string.Empty;
    [DataType(DataType.Date)] public DateOnly? Birthday { get; set; }
    public string? Gender { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Religion { get; set; }

    // Contact
    public string? Address { get; set; }
    public string? MobileNumber { get; set; }
    [EmailAddress] public string? EmailAddress { get; set; }
    public string? HomeNumber { get; set; }

    // Educational background
    [Display(Name = "Learner Reference Number (LRN)")]
    public string? LearnerReferenceNumber { get; set; }
    public string? LastSchoolAttended { get; set; }
    public string? LastLevelObtained { get; set; }

    // Parent / Guardian
    public string? GuardianName { get; set; }
    public string? GuardianContactNumber { get; set; }

    // Application
    [Required(ErrorMessage = "Select the grade level.")]
    public int? GradeLevelId { get; set; }
    [Required(ErrorMessage = "Select the school year.")]
    public int? SchoolYearId { get; set; }
    [Required]
    public string ApplicantType { get; set; } = "New Student";

    public IEnumerable<SelectListItem> GradeLevels { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SchoolYears { get; set; } = new List<SelectListItem>();
}