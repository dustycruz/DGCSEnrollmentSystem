// File: EnrollmentSystem.DAL/Models/AspNetUser.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.DAL.Models;

[Table("AspNetUsers")]
public partial class AspNetUser
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(256)]
    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public bool MustChangePassword { get; set; }

    public bool EmailConfirmed { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();
}