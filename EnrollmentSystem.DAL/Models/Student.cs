using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Student")]
public partial class Student
{
    [Key]
    public int StudentId { get; set; }

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

    public int? Age { get; set; }

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

    [StringLength(20)]
    public string? Extension { get; set; }

    [StringLength(150)]
    public string? GuardianName { get; set; }

    [StringLength(100)]
    public string? GuardianRelationship { get; set; }

    [StringLength(30)]
    public string? GuardianHomeNumber { get; set; }

    [StringLength(30)]
    public string? GuardianOfficeNumber { get; set; }

    [StringLength(150)]
    public string? GuardianEmailAddress { get; set; }

    [StringLength(50)]
    public string? StudentNumber { get; set; }

    public int? ApplicationId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("Students")]
    public virtual Application? Application { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("Student")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("Student")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [InverseProperty("Student")]
    public virtual ICollection<StudentCurriculum> StudentCurricula { get; set; } = new List<StudentCurriculum>();

    [InverseProperty("Student")]
    public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();

    [InverseProperty("Student")]
    public virtual ICollection<StudentSibling> StudentSiblings { get; set; } = new List<StudentSibling>();
}
