// File: EnrollmentSystem.UI/ViewModels/ApplicantPortalViewModel.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class ApplicantPortalViewModel
{
    public Application? Application { get; set; }
    public IEnumerable<Document> RequiredDocuments { get; set; } = new List<Document>();
    public IEnumerable<ApplicationDocument> UploadedDocuments { get; set; } = new List<ApplicationDocument>();
    public IEnumerable<ProofOfPayment> Payments { get; set; } = new List<ProofOfPayment>();
    public PaymentInstructionSettings? PaymentInfo { get; set; }
}