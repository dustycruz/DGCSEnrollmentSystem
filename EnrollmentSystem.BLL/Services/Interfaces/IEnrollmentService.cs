// File: EnrollmentSystem.BLL/Services/Interfaces/IEnrollmentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IEnrollmentService
{
    Task<ServiceResult<int>> EnrollStudentAsync(int studentId, int sectionId, string createdBy);
    Task<IEnumerable<Enrollment>> GetPendingAsync();
    Task<IEnumerable<Enrollment>> GetBySectionAsync(int sectionId);
    Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId);
    Task<Enrollment?> GetAsync(int id);
    Task<ServiceResult> ApproveAsync(int enrollmentId, string modifiedBy);
    Task<ServiceResult> RejectAsync(int enrollmentId, string reason, string modifiedBy);
}