using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Models;

[Table("ProofOfPayment")]
public partial class ProofOfPayment
{
    [Key]
    public int ProofOfPaymentId { get; set; }

    public int ApplicationId { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AmountPaid { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(250)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdateBy { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ProofOfPayments")]
    public virtual Application Application { get; set; } = null!;
}
