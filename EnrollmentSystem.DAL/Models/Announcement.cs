using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Announcement")]
public partial class Announcement
{
    [Key]
    public int AnnouncementId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PostedDate { get; set; }

    public int? SectionId { get; set; }

    public int? SubjectId { get; set; }

    public int? TeacherId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [ForeignKey("SectionId")]
    [InverseProperty("Announcements")]
    public virtual Section? Section { get; set; }

    [ForeignKey("SubjectId")]
    [InverseProperty("Announcements")]
    public virtual Subject? Subject { get; set; }

    [ForeignKey("TeacherId")]
    [InverseProperty("Announcements")]
    public virtual Teacher? Teacher { get; set; }
}
