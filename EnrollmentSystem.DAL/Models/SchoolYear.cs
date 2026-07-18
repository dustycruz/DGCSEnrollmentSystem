using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("SchoolYear")]
public partial class SchoolYear
{
    [Key]
    public int SchoolYearId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("SchoolYear")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [InverseProperty("SchoolYear")]
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();

    [InverseProperty("SchoolYear")]
    public virtual ICollection<StudentCurriculum> StudentCurricula { get; set; } = new List<StudentCurriculum>();
}
