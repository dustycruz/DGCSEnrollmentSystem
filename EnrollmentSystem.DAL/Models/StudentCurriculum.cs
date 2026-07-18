using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("StudentCurriculum")]
public partial class StudentCurriculum
{
    [Key]
    public int StudentCurriculumId { get; set; }

    public int StudentId { get; set; }

    public int CurriculumId { get; set; }

    public int? SchoolYearId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CurriculumId")]
    [InverseProperty("StudentCurricula")]
    public virtual Curriculum Curriculum { get; set; } = null!;

    [ForeignKey("SchoolYearId")]
    [InverseProperty("StudentCurricula")]
    public virtual SchoolYear? SchoolYear { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("StudentCurricula")]
    public virtual Student Student { get; set; } = null!;
}
