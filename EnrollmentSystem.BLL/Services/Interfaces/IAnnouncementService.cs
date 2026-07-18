// File: EnrollmentSystem.BLL/Services/Interfaces/IAnnouncementService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IAnnouncementService
{
    Task<IEnumerable<Announcement>> GetAllAsync();
    Task<IEnumerable<Announcement>> GetRecentAsync(int count);
    Task<IEnumerable<Announcement>> GetBySectionAsync(int sectionId);
    Task<IEnumerable<Announcement>> GetByTeacherAsync(int teacherId);
    Task<ServiceResult<int>> CreateAsync(Announcement announcement, string createdBy);
    Task<ServiceResult> DeleteAsync(int id, string modifiedBy);
}