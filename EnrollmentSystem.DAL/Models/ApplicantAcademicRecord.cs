using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("ApplicantAcademicRecord")]
public partial class ApplicantAcademicRecord
{
    [Key]
    public int ApplicationAcademicRecordId { get; set; }

    public int ApplicationId { get; set; }

    public int? GradeLevelId { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? English { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Math { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Science { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Filipino { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? GeneralAverage { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicantAcademicRecords")]
    public virtual Application Application { get; set; } = null!;

    [ForeignKey("GradeLevelId")]
    [InverseProperty("ApplicantAcademicRecords")]
    public virtual GradeLevel? GradeLevel { get; set; }
}
