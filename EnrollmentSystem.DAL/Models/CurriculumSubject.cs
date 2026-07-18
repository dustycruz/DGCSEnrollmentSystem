using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("CurriculumSubject")]
public partial class CurriculumSubject
{
    [Key]
    public int CurriculumSubjectId { get; set; }

    public int CurriculumId { get; set; }

    public int GradeLevelId { get; set; }

    public int SubjectId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CurriculumId")]
    [InverseProperty("CurriculumSubjects")]
    public virtual Curriculum Curriculum { get; set; } = null!;

    [ForeignKey("GradeLevelId")]
    [InverseProperty("CurriculumSubjects")]
    public virtual GradeLevel GradeLevel { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("CurriculumSubjects")]
    public virtual Subject Subject { get; set; } = null!;
}
