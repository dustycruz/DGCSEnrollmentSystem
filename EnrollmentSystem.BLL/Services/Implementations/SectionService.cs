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
    private readonly IAuditService _audit;

    public SectionService(
        ISectionRepository sectionRepo,
        IScheduleRepository scheduleRepo,
        IGenericRepository<ScheduleDetail> scheduleDetailRepo,
        IAuditService audit)
    {
        _sectionRepo = sectionRepo;
        _scheduleRepo = scheduleRepo;
        _scheduleDetailRepo = scheduleDetailRepo;
        _audit = audit;
    }

    public async Task<IEnumerable<Section>> GetAllAsync() => await _sectionRepo.GetAllActiveAsync();
    public async Task<Section?> GetAsync(int id) => await _sectionRepo.GetByIdAsync(id);
    public async Task<Section?> GetWithSchedulesAsync(int id) => await _sectionRepo.GetWithSchedulesAsync(id);
    public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync() => await _scheduleRepo.GetAllWithDetailsAsync();
    public async Task<Schedule?> GetScheduleAsync(int scheduleId) => await _scheduleRepo.GetFullAsync(scheduleId);

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
        if (detail is not null && !string.IsNullOrWhiteSpace(detail.Day) && detail.StartTime.HasValue && detail.EndTime.HasValue)
        {
            var conflict = await FindConflictAsync(schedule.SectionId, schedule.TeacherId,
                detail.Day, detail.StartTime.Value, detail.EndTime.Value, detail.RoomId, 0);
            if (conflict is not null) return ServiceResult<int>.Fail(conflict);
        }

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
        return ServiceResult<int>.Ok(schedule.ScheduleId, "Schedule added.");
    }

    public async Task<ServiceResult> UpdateScheduleAsync(int scheduleId, int subjectId, int? teacherId, ScheduleDetail detail, string modifiedBy)
    {
        var schedule = await _scheduleRepo.GetFullAsync(scheduleId);
        if (schedule is null) return ServiceResult.Fail("Schedule not found.");

        if (!string.IsNullOrWhiteSpace(detail.Day) && detail.StartTime.HasValue && detail.EndTime.HasValue)
        {
            var conflict = await FindConflictAsync(schedule.SectionId, teacherId,
                detail.Day, detail.StartTime.Value, detail.EndTime.Value, detail.RoomId, scheduleId);
            if (conflict is not null) return ServiceResult.Fail(conflict);
        }

        schedule.SubjectId = subjectId;
        schedule.TeacherId = teacherId;
        schedule.ModifiedBy = modifiedBy;
        _scheduleRepo.Update(schedule);

        var existingDetail = schedule.ScheduleDetails.FirstOrDefault();
        if (existingDetail is not null)
        {
            existingDetail.Day = detail.Day;
            existingDetail.StartTime = detail.StartTime;
            existingDetail.EndTime = detail.EndTime;
            existingDetail.RoomId = detail.RoomId;
            existingDetail.ModifiedBy = modifiedBy;
            _scheduleDetailRepo.Update(existingDetail);
        }
        else
        {
            detail.ScheduleId = scheduleId;
            detail.CreatedBy = modifiedBy;
            detail.IsDeleted = false;
            await _scheduleDetailRepo.AddAsync(detail);
        }

        await _scheduleRepo.SaveAsync();
        return ServiceResult.Ok("Schedule updated.");
    }

    public async Task<ServiceResult> DeleteScheduleAsync(int scheduleId, string modifiedBy)
    {
        var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
        if (schedule is null) return ServiceResult.Fail("Schedule not found.");

        schedule.IsDeleted = true;
        schedule.ModifiedBy = modifiedBy;
        _scheduleRepo.Update(schedule);

        var details = await _scheduleDetailRepo.FindAsync(d => d.ScheduleId == scheduleId);
        foreach (var d in details) { d.IsDeleted = true; _scheduleDetailRepo.Update(d); }

        await _scheduleRepo.SaveAsync();
        return ServiceResult.Ok("Schedule deleted.");
    }

    /// <summary>Returns a message if the section, teacher, or room clashes at an overlapping time; otherwise null.</summary>
    private async Task<string?> FindConflictAsync(int sectionId, int? teacherId, string day,
        TimeOnly start, TimeOnly end, int? roomId, int excludeScheduleId)
    {
        var all = await _scheduleRepo.GetAllWithDetailsAsync();
        foreach (var s in all)
        {
            if (s.ScheduleId == excludeScheduleId) continue;

            foreach (var d in s.ScheduleDetails)
            {
                if (d.Day != day || !d.StartTime.HasValue || !d.EndTime.HasValue) continue;

                var overlaps = start < d.EndTime.Value && d.StartTime.Value < end;
                if (!overlaps) continue;

                if (s.SectionId == sectionId)
                    return $"This section already has a class on {day} at that time ({s.Subject?.Name}).";
                if (teacherId.HasValue && s.TeacherId == teacherId)
                    return $"That teacher is already booked on {day} at that time ({s.Subject?.Name} — {s.Section?.Name}).";
                if (roomId.HasValue && d.RoomId == roomId)
                    return $"That room is already in use on {day} at that time ({s.Subject?.Name} — {s.Section?.Name}).";
            }
        }
        return null;
    }

    public async Task<ServiceResult> AssignAdviserAsync(int sectionId, int? teacherId, string modifiedBy)
    {
        var section = await _sectionRepo.GetByIdAsync(sectionId);
        if (section is null) return ServiceResult.Fail("Section not found.");

        // A teacher may advise only one class — block assigning one who already advises another.
        if (teacherId.HasValue)
        {
            var alreadyAdvising = (await _sectionRepo.GetByAdviserAsync(teacherId.Value))
                .FirstOrDefault(s => s.SectionId != sectionId);
            if (alreadyAdvising is not null)
                return ServiceResult.Fail(
                    $"That teacher is already the adviser of {alreadyAdvising.Name}. Remove them from that class first, or choose a different teacher.");
        }

        section.AdviserTeacherId = teacherId;
        section.ModifiedBy = modifiedBy;
        _sectionRepo.Update(section);
        await _sectionRepo.SaveAsync();

        await _audit.LogAsync(
            action: teacherId is null ? "Adviser Removed" : "Adviser Assigned",
            entityName: "Section",
            entityId: section.SectionId.ToString(),
            description: teacherId is null
                ? $"Adviser removed from {section.Name} by {modifiedBy}."
                : $"Adviser (teacher #{teacherId}) assigned to {section.Name} by {modifiedBy}.",
            status: teacherId is null ? "Removed" : "Assigned");

        return ServiceResult.Ok(teacherId is null ? "Adviser removed." : "Adviser assigned.");
    }
}