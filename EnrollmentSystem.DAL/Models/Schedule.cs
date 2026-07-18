using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Schedule")]
public partial class Schedule
{
    [Key]
    public int ScheduleId { get; set; }

    public int SectionId { get; set; }

    public int SubjectId { get; set; }

    public int? TeacherId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Schedule")]
    public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; } = new List<ScheduleDetail>();

    [ForeignKey("SectionId")]
    [InverseProperty("Schedules")]
    public virtual Section Section { get; set; } = null!;

    [InverseProperty("Schedule")]
    public virtual ICollection<StudentSchedule> StudentSchedules { get; set; } = new List<StudentSchedule>();

    [ForeignKey("SubjectId")]
    [InverseProperty("Schedules")]
    public virtual Subject Subject { get; set; } = null!;

    [ForeignKey("TeacherId")]
    [InverseProperty("Schedules")]
    public virtual Teacher? Teacher { get; set; }
}
