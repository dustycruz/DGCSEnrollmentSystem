// File: EnrollmentSystem.DAL/Repositories/Interfaces/IProofOfPaymentRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IProofOfPaymentRepository : IGenericRepository<ProofOfPayment>
{
    Task<IEnumerable<ProofOfPayment>> GetByApplicationAsync(int applicationId);
    Task<IEnumerable<ProofOfPayment>> GetPendingAsync();
    Task<ProofOfPayment?> GetWithApplicationAsync(int proofOfPaymentId);
    Task<IEnumerable<ProofOfPayment>> GetByStudentAsync(int studentId);
}