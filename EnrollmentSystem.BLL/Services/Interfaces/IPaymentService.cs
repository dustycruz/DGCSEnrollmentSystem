// File: EnrollmentSystem.BLL/Services/Interfaces/IPaymentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentRowDto>> GetAllAsync(string? status);
    Task<ServiceResult> ReviewAsync(int proofId, string status, string? remarks, string reviewedBy);
}