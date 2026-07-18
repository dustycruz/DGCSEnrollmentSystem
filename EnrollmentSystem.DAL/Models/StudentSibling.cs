using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

public partial class StudentSibling
{
    [Key]
    public int StudentSiblingId { get; set; }

    public int StudentId { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    public int? Age { get; set; }

    [StringLength(200)]
    public string? School { get; set; }

    [StringLength(100)]
    public string? YearLevel { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("StudentSiblings")]
    public virtual Student Student { get; set; } = null!;
}
