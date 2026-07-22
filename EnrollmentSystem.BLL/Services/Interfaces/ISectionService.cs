// File: EnrollmentSystem.BLL/Services/Interfaces/ISectionService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface ISectionService
{
    Task<IEnumerable<Section>> GetAllAsync();
    Task<Section?> GetAsync(int id);
    Task<Section?> GetWithSchedulesAsync(int id);
    Task<ServiceResult<int>> CreateAsync(Section section, string createdBy);
    Task<ServiceResult> UpdateAsync(Section section, string modifiedBy);
    Task<ServiceResult> DeleteAsync(int id, string modifiedBy);
    Task<ServiceResult<int>> AddScheduleAsync(Schedule schedule, ScheduleDetail? detail, string createdBy);
    Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
    Task<Schedule?> GetScheduleAsync(int scheduleId);
    Task<ServiceResult> UpdateScheduleAsync(int scheduleId, int subjectId, int? teacherId, ScheduleDetail detail, string modifiedBy);
    Task<ServiceResult> DeleteScheduleAsync(int scheduleId, string modifiedBy);
    Task<ServiceResult> AssignAdviserAsync(int sectionId, int? teacherId, string modifiedBy);
}