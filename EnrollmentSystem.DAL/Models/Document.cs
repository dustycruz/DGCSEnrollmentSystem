using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

public partial class Document
{
    [Key]
    public int DocumentId { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public int? EducationalLevelId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Document")]
    public virtual ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = new List<ApplicationDocument>();

    [ForeignKey("EducationalLevelId")]
    [InverseProperty("Documents")]
    public virtual EducationalLevel? EducationalLevel { get; set; }
}
