// File: EnrollmentSystem.BLL/Services/Implementations/PaymentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IProofOfPaymentRepository _proofRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notification;
    private readonly IEmailService _email;

    public PaymentService(
        IProofOfPaymentRepository proofRepo,
        IStudentRepository studentRepo,
        IUserRepository userRepo,
        INotificationService notification,
        IEmailService email)
    {
        _proofRepo = proofRepo;
        _studentRepo = studentRepo;
        _userRepo = userRepo;
        _notification = notification;
        _email = email;
    }

    public async Task<IEnumerable<PaymentRowDto>> GetAllAsync(string? status)
    {
        var payments = (await _proofRepo.GetAllWithDetailsAsync()).ToList();
        if (!string.IsNullOrWhiteSpace(status) && status != "All")
            payments = payments.Where(p => p.Status == status).ToList();

        var rows = new List<PaymentRowDto>();
        foreach (var p in payments)
        {
            string payer, type;
            if (p.StudentId.HasValue)
            {
                var st = await _studentRepo.GetByIdAsync(p.StudentId.Value);
                payer = st is null ? "Unknown" : $"{st.LastName}, {st.FirstName} ({st.StudentNumber})";
                type = "Student";
            }
            else if (p.Application is not null)
            {
                payer = $"{p.Application.LastName}, {p.Application.FirstName}";
                type = "Applicant";
            }
            else { payer = "Unknown"; type = "-"; }

            rows.Add(new PaymentRowDto
            {
                ProofOfPaymentId = p.ProofOfPaymentId,
                PayerName = payer,
                PayerType = type,
                Reference = p.ReferenceNumber,
                Amount = p.AmountPaid,
                Method = p.PaymentMethod,
                Purpose = p.Purpose,
                Date = p.PaymentDate,
                Status = p.Status,
                Remarks = p.Remarks,
                FilePath = p.FilePath,
                ReviewedBy = p.ModifiedBy
            });
        }
        return rows;
    }

    public async Task<ServiceResult> ReviewAsync(int proofId, string status, string? remarks, string reviewedBy)
    {
        var proof = await _proofRepo.GetWithApplicationAsync(proofId);
        if (proof is null) return ServiceResult.Fail("Payment not found.");

        proof.Status = status;
        proof.Remarks = remarks;
        proof.ModifiedBy = reviewedBy;
        _proofRepo.Update(proof);
        await _proofRepo.SaveAsync();

        var message = $"Your payment was {status}." + (string.IsNullOrWhiteSpace(remarks) ? "" : $" Remarks: {remarks}");

        if (proof.StudentId.HasValue)
        {
            var st = await _studentRepo.GetByIdAsync(proof.StudentId.Value);
            if (st is not null)
            {
                if (!string.IsNullOrWhiteSpace(st.StudentNumber))
                {
                    var user = await _userRepo.GetByUsernameAsync(st.StudentNumber);
                    if (user is not null) await _notification.NotifyAsync(user.Id, $"Payment {status}", message);
                }
                if (!string.IsNullOrWhiteSpace(st.EmailAddress))
                    await _email.SendStatusUpdateAsync(st.EmailAddress, $"Payment {status}", remarks);
            }
        }
        else if (proof.Application is not null)
        {
            var app = proof.Application;
            if (!string.IsNullOrWhiteSpace(app.CreatedBy))
            {
                var user = await _userRepo.GetByUsernameAsync(app.CreatedBy);
                if (user is not null) await _notification.NotifyAsync(user.Id, $"Payment {status}", message);
            }
            if (!string.IsNullOrWhiteSpace(app.EmailAddress))
                await _email.SendStatusUpdateAsync(app.EmailAddress, $"Payment {status}", remarks);
        }

        return ServiceResult.Ok($"Payment marked as {status}.");
    }
}