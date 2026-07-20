// File: EnrollmentSystem.BLL/Services/Interfaces/IProofOfPaymentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IProofOfPaymentService
{
    Task<ServiceResult<int>> SubmitAsync(ProofOfPayment proof, string createdBy);
    Task<IEnumerable<ProofOfPayment>> GetByApplicationAsync(int applicationId);
    Task<IEnumerable<ProofOfPayment>> GetPendingAsync();
    Task<ServiceResult> VerifyAsync(int id, string status, string? remarks, string modifiedBy);
    Task<IEnumerable<ProofOfPayment>> GetByStudentAsync(int studentId);
}