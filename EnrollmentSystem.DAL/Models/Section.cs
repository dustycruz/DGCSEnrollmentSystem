using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Section")]
public partial class Section
{
    [Key]
    public int SectionId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int? SchoolYearId { get; set; }

    public int? GradeLevelId { get; set; }

    public int? CurriculumId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    [ForeignKey("CurriculumId")]
    [InverseProperty("Sections")]
    public virtual Curriculum? Curriculum { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [ForeignKey("GradeLevelId")]
    [InverseProperty("Sections")]
    public virtual GradeLevel? GradeLevel { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [ForeignKey("SchoolYearId")]
    [InverseProperty("Sections")]
    public virtual SchoolYear? SchoolYear { get; set; }
}
