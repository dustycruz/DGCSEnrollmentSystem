using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

public partial class ScheduleDetail
{
    [Key]
    public int ScheduleDetailId { get; set; }

    public int ScheduleId { get; set; }

    [StringLength(20)]
    public string? Day { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? RoomId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("ScheduleDetails")]
    public virtual Room? Room { get; set; }

    [ForeignKey("ScheduleId")]
    [InverseProperty("ScheduleDetails")]
    public virtual Schedule Schedule { get; set; } = null!;
}
