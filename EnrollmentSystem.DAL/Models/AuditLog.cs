using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("AuditLog")]
public partial class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }

    public int? StudentId { get; set; }

    [StringLength(100)]
    public string? Action { get; set; }

    [StringLength(150)]
    public string? EntityName { get; set; }

    [StringLength(100)]
    public string? EntityId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    [StringLength(150)]
    public string? TableName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public string? Details { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("AuditLogs")]
    public virtual Student? Student { get; set; }
}
