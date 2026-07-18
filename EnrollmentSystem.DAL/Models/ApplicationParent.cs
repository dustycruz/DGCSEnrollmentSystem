using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("ApplicationParent")]
public partial class ApplicationParent
{
    [Key]
    public int ApplicationParentId { get; set; }

    public int ApplicationId { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(30)]
    public string? HomeContactNumber { get; set; }

    [StringLength(30)]
    public string? MobileNumber { get; set; }

    [StringLength(150)]
    public string? EmailAddress { get; set; }

    [StringLength(150)]
    public string? Occupation { get; set; }

    [StringLength(150)]
    public string? CompanyName { get; set; }

    [StringLength(30)]
    public string? BusinessContactNumber { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicationParents")]
    public virtual Application Application { get; set; } = null!;
}
