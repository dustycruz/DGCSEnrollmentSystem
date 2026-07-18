using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("ApplicationDocument")]
public partial class ApplicationDocument
{
    [Key]
    public int ApplicationDocumentId { get; set; }

    public int ApplicationId { get; set; }

    [StringLength(500)]
    public string? Url { get; set; }

    public int? DocumentId { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicationDocuments")]
    public virtual Application Application { get; set; } = null!;

    [ForeignKey("DocumentId")]
    [InverseProperty("ApplicationDocuments")]
    public virtual Document? Document { get; set; }
}
