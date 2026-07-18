// File: EnrollmentSystem.BLL/Services/Implementations/ApplicationService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepo;
    private readonly IApplicationDocumentRepository _documentRepo;

    public ApplicationService(
        IApplicationRepository applicationRepo,
        IApplicationDocumentRepository documentRepo)
    {
        _applicationRepo = applicationRepo;
        _documentRepo = documentRepo;
    }

    public async Task<ServiceResult<int>> SubmitApplicationAsync(Application application, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(application.FirstName) || string.IsNullOrWhiteSpace(application.LastName))
            return ServiceResult<int>.Fail("First name and last name are required.");

        application.Status = "Pending";
        application.ApplicationDate = DateTime.Now;
        application.CreatedBy = createdBy;
        application.IsDeleted = false;

        await _applicationRepo.AddAsync(application);
        await _applicationRepo.SaveAsync();

        return ServiceResult<int>.Ok(application.ApplicationId, "Application submitted successfully.");
    }

    public async Task<IEnumerable<Application>> GetMyApplicationsAsync(string createdBy)
        => await _applicationRepo.GetByCreatedByAsync(createdBy);

    public async Task<IEnumerable<Application>> GetAllAsync()
        => await _applicationRepo.GetAllActiveAsync();

    public async Task<IEnumerable<Application>> GetPendingAsync()
        => await _applicationRepo.GetPendingAsync();

    public async Task<Application?> GetApplicationAsync(int id)
        => await _applicationRepo.GetFullApplicationAsync(id);

    public async Task<ServiceResult> UpdateStatusAsync(int id, string status, string modifiedBy)
    {
        var application = await _applicationRepo.GetByIdAsync(id);
        if (application is null)
            return ServiceResult.Fail("Application not found.");

        application.Status = status;
        application.ModifiedBy = modifiedBy;
        _applicationRepo.Update(application);
        await _applicationRepo.SaveAsync();

        return ServiceResult.Ok($"Application marked as {status}.");
    }

    public async Task<ServiceResult<int>> AddDocumentAsync(int applicationId, string url, int? documentId)
    {
        var doc = new ApplicationDocument
        {
            ApplicationId = applicationId,
            Url = url,
            DocumentId = documentId
        };

        await _documentRepo.AddAsync(doc);
        await _documentRepo.SaveAsync();

        return ServiceResult<int>.Ok(doc.ApplicationDocumentId, "Document uploaded.");
    }

    public async Task<IEnumerable<ApplicationDocument>> GetDocumentsAsync(int applicationId)
        => await _documentRepo.GetByApplicationAsync(applicationId);
}