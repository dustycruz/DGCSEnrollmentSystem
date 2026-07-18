using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("GradeLevel")]
public partial class GradeLevel
{
    [Key]
    public int GradeLevelId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int EducationalLevelId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("GradeLevel")]
    public virtual ICollection<ApplicantAcademicRecord> ApplicantAcademicRecords { get; set; } = new List<ApplicantAcademicRecord>();

    [InverseProperty("GradeLevel")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [InverseProperty("GradeLevel")]
    public virtual ICollection<CurriculumSubject> CurriculumSubjects { get; set; } = new List<CurriculumSubject>();

    [ForeignKey("EducationalLevelId")]
    [InverseProperty("GradeLevels")]
    public virtual EducationalLevel EducationalLevel { get; set; } = null!;

    [InverseProperty("GradeLevel")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [InverseProperty("GradeLevel")]
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
}
