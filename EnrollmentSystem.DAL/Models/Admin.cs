using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("Admin")]
public partial class Admin
{
    [Key]
    public int AdminId { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Admins")]
    public virtual Employee Employee { get; set; } = null!;
}
