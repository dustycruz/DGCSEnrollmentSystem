using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Teacher")]
public partial class Teacher
{
    [Key]
    public int TeacherId { get; set; }

    public int EmployeeId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("Teachers")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("Teacher")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
