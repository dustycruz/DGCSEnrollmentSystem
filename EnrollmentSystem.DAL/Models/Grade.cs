using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

public partial class Grade
{
    [Key]
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    [StringLength(20)]
    public string? Quarter { get; set; }

    [Column("Grade", TypeName = "decimal(5, 2)")]
    public decimal? Grade1 { get; set; }

    public int? GradeLevelId { get; set; }

    public int? TeacherId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("GradeLevelId")]
    [InverseProperty("Grades")]
    public virtual GradeLevel? GradeLevel { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("Grades")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("Grades")]
    public virtual Subject Subject { get; set; } = null!;

    [ForeignKey("TeacherId")]
    [InverseProperty("Grades")]
    public virtual Teacher? Teacher { get; set; }
}
