// File: EnrollmentSystem.BLL/Services/Implementations/SectionService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepo;
    private readonly IScheduleRepository _scheduleRepo;
    private readonly IGenericRepository<ScheduleDetail> _scheduleDetailRepo;

    public SectionService(
        ISectionRepository sectionRepo,
        IScheduleRepository scheduleRepo,
        IGenericRepository<ScheduleDetail> scheduleDetailRepo)
    {
        _sectionRepo = sectionRepo;
        _scheduleRepo = scheduleRepo;
        _scheduleDetailRepo = scheduleDetailRepo;
    }

    public async Task<IEnumerable<Section>> GetAllAsync() => await _sectionRepo.GetAllActiveAsync();
    public async Task<Section?> GetAsync(int id) => await _sectionRepo.GetByIdAsync(id);
    public async Task<Section?> GetWithSchedulesAsync(int id) => await _sectionRepo.GetWithSchedulesAsync(id);

    public async Task<ServiceResult<int>> CreateAsync(Section section, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(section.Name))
            return ServiceResult<int>.Fail("Section name is required.");

        section.CreatedBy = createdBy;
        section.IsDeleted = false;
        await _sectionRepo.AddAsync(section);
        await _sectionRepo.SaveAsync();
        return ServiceResult<int>.Ok(section.SectionId, "Section created.");
    }

    public async Task<ServiceResult> UpdateAsync(Section section, string modifiedBy)
    {
        var existing = await _sectionRepo.GetByIdAsync(section.SectionId);
        if (existing is null) return ServiceResult.Fail("Section not found.");

        existing.Name = section.Name;
        existing.SchoolYearId = section.SchoolYearId;
        existing.GradeLevelId = section.GradeLevelId;
        existing.CurriculumId = section.CurriculumId;
        existing.ModifiedBy = modifiedBy;
        _sectionRepo.Update(existing);
        await _sectionRepo.SaveAsync();
        return ServiceResult.Ok("Section updated.");
    }

    public async Task<ServiceResult> DeleteAsync(int id, string modifiedBy)
    {
        var existing = await _sectionRepo.GetByIdAsync(id);
        if (existing is null) return ServiceResult.Fail("Section not found.");

        existing.IsDeleted = true;
        existing.ModifiedBy = modifiedBy;
        _sectionRepo.Update(existing);
        await _sectionRepo.SaveAsync();
        return ServiceResult.Ok("Section deleted.");
    }

    public async Task<ServiceResult<int>> AddScheduleAsync(Schedule schedule, ScheduleDetail? detail, string createdBy)
    {
        schedule.CreatedBy = createdBy;
        schedule.IsDeleted = false;
        await _scheduleRepo.AddAsync(schedule);
        await _scheduleRepo.SaveAsync();

        if (detail is not null)
        {
            detail.ScheduleId = schedule.ScheduleId;
            detail.CreatedBy = createdBy;
            detail.IsDeleted = false;
            await _scheduleDetailRepo.AddAsync(detail);
            await _scheduleDetailRepo.SaveAsync();
        }

        return ServiceResult<int>.Ok(schedule.ScheduleId, "Schedule added to section.");
    }
}