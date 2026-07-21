// File: EnrollmentSystem.BLL/DTOs/PaymentRowDto.cs
namespace EnrollmentSystem.BLL.DTOs;

public class PaymentRowDto
{
    public int ProofOfPaymentId { get; set; }
    public string PayerName { get; set; } = string.Empty;
    public string PayerType { get; set; } = string.Empty;   // Student / Applicant
    public string? Reference { get; set; }
    public decimal? Amount { get; set; }
    public string? Method { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? FilePath { get; set; }
    public string? ReviewedBy { get; set; }
}