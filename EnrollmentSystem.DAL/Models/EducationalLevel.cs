using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("EducationalLevel")]
public partial class EducationalLevel
{
    [Key]
    public int EducationalLevelId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("EducationalLevel")]
    public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();

    [InverseProperty("EducationalLevel")]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [InverseProperty("EducationalLevel")]
    public virtual ICollection<GradeLevel> GradeLevels { get; set; } = new List<GradeLevel>();
}
