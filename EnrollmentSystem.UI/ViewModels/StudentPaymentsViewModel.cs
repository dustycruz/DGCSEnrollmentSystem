// File: EnrollmentSystem.UI/ViewModels/StudentPaymentsViewModel.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class StudentPaymentsViewModel
{
    public PaymentInstructionSettings? Info { get; set; }
    public IEnumerable<ProofOfPayment> Payments { get; set; } = new List<ProofOfPayment>();
    public decimal TotalFee { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
}