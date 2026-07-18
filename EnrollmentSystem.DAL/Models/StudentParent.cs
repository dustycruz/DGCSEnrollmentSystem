using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("StudentParent")]
public partial class StudentParent
{
    [Key]
    public int StudentParentId { get; set; }

    public int StudentId { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(30)]
    public string? HomeContactNumber { get; set; }

    [StringLength(30)]
    public string? MobileNumber { get; set; }

    [StringLength(150)]
    public string? EmailAddress { get; set; }

    [StringLength(150)]
    public string? Position { get; set; }

    [StringLength(150)]
    public string? CompanyName { get; set; }

    [StringLength(30)]
    public string? OfficeNumber { get; set; }

    public DateOnly? Birthday { get; set; }

    [StringLength(100)]
    public string? Citizenship { get; set; }

    [StringLength(100)]
    public string? Religion { get; set; }

    [StringLength(150)]
    public string? EducationalAttainment { get; set; }

    [StringLength(150)]
    public string? Course { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("StudentParents")]
    public virtual Student Student { get; set; } = null!;
}
