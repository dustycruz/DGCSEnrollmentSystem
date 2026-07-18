using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Application")]
public partial class Application
{
    [Key]
    public int ApplicationId { get; set; }

    public int? SchoolYearId { get; set; }

    public int? GradeLevelId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(30)]
    public string? MobileNumber { get; set; }

    [StringLength(150)]
    public string? GuardianName { get; set; }

    [StringLength(30)]
    public string? GuardianContactNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ApplicationDate { get; set; }

    [StringLength(50)]
    public string? LearnerReferenceNumber { get; set; }

    [StringLength(150)]
    public string? PlaceOfBirth { get; set; }

    [StringLength(100)]
    public string? Religion { get; set; }

    [StringLength(150)]
    public string? EmailAddress { get; set; }

    [StringLength(30)]
    public string? HomeNumber { get; set; }

    [StringLength(100)]
    public string? ResidesWith { get; set; }

    [StringLength(200)]
    public string? LastSchoolAttended { get; set; }

    [StringLength(100)]
    public string? LastLevelObtained { get; set; }

    [StringLength(100)]
    public string? ParentStatus { get; set; }

    [StringLength(200)]
    public string? Sports { get; set; }

    [StringLength(200)]
    public string? Hobbies { get; set; }

    [StringLength(200)]
    public string? Illness { get; set; }

    public bool? IsVarsityPlayer { get; set; }

    public bool? HaveOrganization { get; set; }

    [StringLength(200)]
    public string? Organization { get; set; }

    [StringLength(200)]
    public string? PursueInCollege { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<ApplicantAcademicRecord> ApplicantAcademicRecords { get; set; } = new List<ApplicantAcademicRecord>();

    [InverseProperty("Application")]
    public virtual ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();

    [InverseProperty("Application")]
    public virtual ICollection<ApplicationParent> ApplicationParents { get; set; } = new List<ApplicationParent>();

    [InverseProperty("Application")]
    public virtual ICollection<ApplicationSibling> ApplicationSiblings { get; set; } = new List<ApplicationSibling>();

    [ForeignKey("GradeLevelId")]
    [InverseProperty("Applications")]
    public virtual GradeLevel? GradeLevel { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<ProofOfPayment> ProofOfPayments { get; set; } = new List<ProofOfPayment>();

    [ForeignKey("SchoolYearId")]
    [InverseProperty("Applications")]
    public virtual SchoolYear? SchoolYear { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
