using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

public partial class ApplicationSibling
{
    [Key]
    public int ApplicationSiblingId { get; set; }

    public int ApplicationId { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    public int? Age { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

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

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicationSiblings")]
    public virtual Application Application { get; set; } = null!;
}
