using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Curriculum")]
public partial class Curriculum
{
    [Key]
    public int CurriculumId { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    public int EducationalLevelId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Curriculum")]
    public virtual ICollection<CurriculumSubject> CurriculumSubjects { get; set; } = new List<CurriculumSubject>();

    [ForeignKey("EducationalLevelId")]
    [InverseProperty("Curricula")]
    public virtual EducationalLevel EducationalLevel { get; set; } = null!;

    [InverseProperty("Curriculum")]
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();

    [InverseProperty("Curriculum")]
    public virtual ICollection<StudentCurriculum> StudentCurricula { get; set; } = new List<StudentCurriculum>();
}
