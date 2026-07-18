// File: EnrollmentSystem.BLL/Services/Implementations/SubjectService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepo;

    public SubjectService(ISubjectRepository subjectRepo)
    {
        _subjectRepo = subjectRepo;
    }

    public async Task<IEnumerable<Subject>> GetAllAsync()
        => await _subjectRepo.GetAllActiveAsync();

    public async Task<Subject?> GetAsync(int id)
        => await _subjectRepo.GetByIdAsync(id);

    public async Task<ServiceResult<int>> CreateAsync(Subject subject, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(subject.Name))
            return ServiceResult<int>.Fail("Subject name is required.");

        subject.CreatedBy = createdBy;
        subject.IsDeleted = false;

        await _subjectRepo.AddAsync(subject);
        await _subjectRepo.SaveAsync();

        return ServiceResult<int>.Ok(subject.SubjectId, "Subject created.");
    }

    public async Task<ServiceResult> UpdateAsync(Subject subject, string modifiedBy)
    {
        var existing = await _subjectRepo.GetByIdAsync(subject.SubjectId);
        if (existing is null)
            return ServiceResult.Fail("Subject not found.");

        existing.Name = subject.Name;
        existing.Code = subject.Code;
        existing.ModifiedBy = modifiedBy;

        _subjectRepo.Update(existing);
        await _subjectRepo.SaveAsync();

        return ServiceResult.Ok("Subject updated.");
    }

    public async Task<ServiceResult> DeleteAsync(int id, string modifiedBy)
    {
        var existing = await _subjectRepo.GetByIdAsync(id);
        if (existing is null)
            return ServiceResult.Fail("Subject not found.");

        existing.IsDeleted = true;
        existing.ModifiedBy = modifiedBy;
        _subjectRepo.Update(existing);
        await _subjectRepo.SaveAsync();

        return ServiceResult.Ok("Subject deleted.");
    }
}