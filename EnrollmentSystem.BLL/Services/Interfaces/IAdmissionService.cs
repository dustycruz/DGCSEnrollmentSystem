// File: EnrollmentSystem.BLL/Services/Interfaces/IAdmissionService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IAdmissionService
{
    Task<IEnumerable<Application>> GetQueueAsync(string? status);
    Task<Application?> GetFullAsync(int id);
    Task<ServiceResult> ReviewAsync(int applicationId, string newStatus, string? remarks, string reviewedBy);
    Task<ServiceResult> VerifyPaymentAsync(int proofId, string status, string? remarks, string reviewedBy);
}