// File: EnrollmentSystem.BLL/Services/Implementations/AdmissionService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AdmissionService : IAdmissionService
{
    private readonly IApplicationRepository _applicationRepo;
    private readonly IProofOfPaymentRepository _proofRepo;
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notification;
    private readonly IEmailService _email;
    private readonly IAuditService _audit;

    public AdmissionService(
        IApplicationRepository applicationRepo,
        IProofOfPaymentRepository proofRepo,
        IUserRepository userRepo,
        INotificationService notification,
        IEmailService email,
        IAuditService audit)
    {
        _applicationRepo = applicationRepo;
        _proofRepo = proofRepo;
        _userRepo = userRepo;
        _notification = notification;
        _email = email;
        _audit = audit;
    }

    public async Task<IEnumerable<Application>> GetQueueAsync(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status == "All")
            return await _applicationRepo.GetAllActiveAsync();
        return await _applicationRepo.GetByStatusAsync(status);
    }

    public async Task<Application?> GetFullAsync(int id)
        => await _applicationRepo.GetFullApplicationAsync(id);

    public async Task<ServiceResult> ReviewAsync(int applicationId, string newStatus, string? remarks, string reviewedBy)
    {
        var app = await _applicationRepo.GetByIdAsync(applicationId);
        if (app is null) return ServiceResult.Fail("Application not found.");

        // Once enrolled, the application is finalized and locked from further changes.
        if (app.Status == ApplicationStatuses.Enrolled)
            return ServiceResult.Fail("This application is already enrolled and can no longer be changed.");

        app.Status = newStatus;
        app.Remarks = remarks;
        app.ModifiedBy = reviewedBy;
        _applicationRepo.Update(app);
        await _applicationRepo.SaveAsync();

        await NotifyApplicantAsync(app,
            $"Application: {newStatus}",
            $"Your application status is now {newStatus}." +
                (string.IsNullOrWhiteSpace(remarks) ? "" : $" Remarks: {remarks}"),
            newStatus, remarks);

        await _audit.LogAsync(
            action: $"Application {newStatus}",
            entityName: "Application",
            entityId: app.ApplicationId.ToString(),
            description: $"Application of {app.LastName}, {app.FirstName} marked {newStatus} by {reviewedBy}.",
            status: newStatus,
            details: remarks);

        return ServiceResult.Ok($"Application marked as {newStatus}.");
    }

    public async Task<ServiceResult> VerifyPaymentAsync(int proofId, string status, string? remarks, string reviewedBy)
    {
        var proof = await _proofRepo.GetWithApplicationAsync(proofId);
        if (proof is null) return ServiceResult.Fail("Payment record not found.");

        proof.Status = status;
        proof.Remarks = remarks;
        proof.ModifiedBy = reviewedBy;
        _proofRepo.Update(proof);
        await _proofRepo.SaveAsync();

        if (proof.Application is not null)
            await NotifyApplicantAsync(proof.Application,
                $"Payment {status}",
                $"Your proof of payment was {status}." +
                    (string.IsNullOrWhiteSpace(remarks) ? "" : $" Remarks: {remarks}"),
                null, null);

        await _audit.LogAsync(
            action: $"Payment {status}",
            entityName: "ProofOfPayment",
            entityId: proof.ProofOfPaymentId.ToString(),
            description: $"Applicant payment (ref {proof.ReferenceNumber}) marked {status} by {reviewedBy}.",
            status: status,
            details: remarks);

        return ServiceResult.Ok($"Payment marked as {status}.");
    }

    private async Task NotifyApplicantAsync(Application app, string title, string message, string? emailStatus, string? emailRemarks)
    {
        if (!string.IsNullOrWhiteSpace(app.CreatedBy))
        {
            var user = await _userRepo.GetByUsernameAsync(app.CreatedBy);
            if (user is not null)
                await _notification.NotifyAsync(user.Id, title, message);
        }

        if (!string.IsNullOrWhiteSpace(app.EmailAddress) && emailStatus is not null)
            await _email.SendStatusUpdateAsync(app.EmailAddress, emailStatus, emailRemarks);
    }

}