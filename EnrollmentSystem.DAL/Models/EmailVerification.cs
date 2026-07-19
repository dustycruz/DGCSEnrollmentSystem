// File: EnrollmentSystem.DAL/Models/EmailVerification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.DAL.Models;

[Table("EmailVerification")]
public partial class EmailVerification
{
    [Key]
    public int EmailVerificationId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(10)]
    public string Code { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    public bool IsVerified { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}