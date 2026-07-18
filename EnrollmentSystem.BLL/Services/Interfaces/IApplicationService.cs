// File: EnrollmentSystem.BLL/Services/Interfaces/IApplicationService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IApplicationService
{
    Task<ServiceResult<int>> SubmitApplicationAsync(Application application, string createdBy);
    Task<IEnumerable<Application>> GetMyApplicationsAsync(string createdBy);
    Task<IEnumerable<Application>> GetAllAsync();
    Task<IEnumerable<Application>> GetPendingAsync();
    Task<Application?> GetApplicationAsync(int id);
    Task<ServiceResult> UpdateStatusAsync(int id, string status, string modifiedBy);
    Task<ServiceResult<int>> AddDocumentAsync(int applicationId, string url, int? documentId);
    Task<IEnumerable<ApplicationDocument>> GetDocumentsAsync(int applicationId);
}