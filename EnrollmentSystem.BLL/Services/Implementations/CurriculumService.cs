// File: EnrollmentSystem.BLL/Services/Implementations/CurriculumService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class CurriculumService : ICurriculumService
{
    private readonly ICurriculumRepository _curriculumRepo;
    private readonly IGenericRepository<CurriculumSubject> _curriculumSubjectRepo;

    public CurriculumService(
        ICurriculumRepository curriculumRepo,
        IGenericRepository<CurriculumSubject> curriculumSubjectRepo)
    {
        _curriculumRepo = curriculumRepo;
        _curriculumSubjectRepo = curriculumSubjectRepo;
    }

    public async Task<IEnumerable<Curriculum>> GetAllAsync()
        => await _curriculumRepo.GetAllActiveAsync();

    public async Task<Curriculum?> GetWithSubjectsAsync(int id)
        => await _curriculumRepo.GetWithSubjectsAsync(id);

    public async Task<ServiceResult<int>> CreateAsync(Curriculum curriculum, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(curriculum.Code))
            return ServiceResult<int>.Fail("Curriculum code is required.");

        curriculum.CreatedBy = createdBy;
        curriculum.IsDeleted = false;

        await _curriculumRepo.AddAsync(curriculum);
        await _curriculumRepo.SaveAsync();

        return ServiceResult<int>.Ok(curriculum.CurriculumId, "Curriculum created.");
    }

    public async Task<ServiceResult<int>> AddSubjectAsync(CurriculumSubject curriculumSubject, string createdBy)
    {
        curriculumSubject.CreatedBy = createdBy;
        curriculumSubject.IsDeleted = false;

        await _curriculumSubjectRepo.AddAsync(curriculumSubject);
        await _curriculumSubjectRepo.SaveAsync();

        return ServiceResult<int>.Ok(curriculumSubject.CurriculumSubjectId, "Subject added to curriculum.");
    }

    public async Task<ServiceResult> DeleteAsync(int id, string modifiedBy)
    {
        var existing = await _curriculumRepo.GetByIdAsync(id);
        if (existing is null)
            return ServiceResult.Fail("Curriculum not found.");

        existing.IsDeleted = true;
        existing.ModifiedBy = modifiedBy;
        _curriculumRepo.Update(existing);
        await _curriculumRepo.SaveAsync();

        return ServiceResult.Ok("Curriculum deleted.");
    }
}