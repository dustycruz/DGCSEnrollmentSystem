// File: EnrollmentSystem.DAL/Models/Notification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.DAL.Models;

[Table("Notification")]
public partial class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Message { get; set; }

    public bool IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}