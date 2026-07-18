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
    Task<ServiceResult<int>> AddScheduleAsync(Schedule schedule, string createdBy);
}