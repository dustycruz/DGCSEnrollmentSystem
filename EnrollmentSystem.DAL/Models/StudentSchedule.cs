using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("StudentSchedule")]
public partial class StudentSchedule
{
    [Key]
    public int StudentScheduleId { get; set; }

    public int EnrollmentId { get; set; }

    public int ScheduleId { get; set; }

    [ForeignKey("EnrollmentId")]
    [InverseProperty("StudentSchedules")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [ForeignKey("ScheduleId")]
    [InverseProperty("StudentSchedules")]
    public virtual Schedule Schedule { get; set; } = null!;
}
