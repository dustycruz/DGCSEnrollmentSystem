// File: EnrollmentSystem.DAL/Repositories/Implementations/ProofOfPaymentRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class ProofOfPaymentRepository : GenericRepository<ProofOfPayment>, IProofOfPaymentRepository
{
    public ProofOfPaymentRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<ProofOfPayment>> GetByApplicationAsync(int applicationId)
        => await _dbSet.AsNoTracking()
            .Where(p => !p.IsDeleted && p.ApplicationId == applicationId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

    public async Task<IEnumerable<ProofOfPayment>> GetPendingAsync()
        => await _dbSet.AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == "Pending")
            .Include(p => p.Application)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

    public async Task<ProofOfPayment?> GetWithApplicationAsync(int proofOfPaymentId)
        => await _dbSet
            .Include(p => p.Application)
            .FirstOrDefaultAsync(p => p.ProofOfPaymentId == proofOfPaymentId && !p.IsDeleted);
}