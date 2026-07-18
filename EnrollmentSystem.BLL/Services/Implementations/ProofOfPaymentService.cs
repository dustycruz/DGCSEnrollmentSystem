// File: EnrollmentSystem.BLL/Services/Implementations/ProofOfPaymentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class ProofOfPaymentService : IProofOfPaymentService
{
    private readonly IProofOfPaymentRepository _proofRepo;

    public ProofOfPaymentService(IProofOfPaymentRepository proofRepo)
    {
        _proofRepo = proofRepo;
    }

    public async Task<ServiceResult<int>> SubmitAsync(ProofOfPayment proof, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(proof.FilePath))
            return ServiceResult<int>.Fail("A proof-of-payment file is required.");

        proof.Status = "Pending";
        proof.PaymentDate ??= DateTime.Now;
        proof.CreatedBy = createdBy;
        proof.IsDeleted = false;

        await _proofRepo.AddAsync(proof);
        await _proofRepo.SaveAsync();

        return ServiceResult<int>.Ok(proof.ProofOfPaymentId, "Proof of payment submitted.");
    }

    public async Task<IEnumerable<ProofOfPayment>> GetByApplicationAsync(int applicationId)
        => await _proofRepo.GetByApplicationAsync(applicationId);

    public async Task<IEnumerable<ProofOfPayment>> GetPendingAsync()
        => await _proofRepo.GetPendingAsync();

    public async Task<ServiceResult> VerifyAsync(int id, string status, string? remarks, string modifiedBy)
    {
        var proof = await _proofRepo.GetByIdAsync(id);
        if (proof is null)
            return ServiceResult.Fail("Proof of payment not found.");

        proof.Status = status;
        proof.Remarks = remarks;
        proof.ModifiedBy = modifiedBy;
        _proofRepo.Update(proof);
        await _proofRepo.SaveAsync();

        return ServiceResult.Ok($"Payment marked as {status}.");
    }
}