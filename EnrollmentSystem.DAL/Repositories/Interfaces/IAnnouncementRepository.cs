// File: EnrollmentSystem.DAL/Repositories/Interfaces/IAnnouncementRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IAnnouncementRepository : IGenericRepository<Announcement>
{
    Task<IEnumerable<Announcement>> GetRecentAsync(int count);
    Task<IEnumerable<Announcement>> GetBySectionAsync(int sectionId);
    Task<IEnumerable<Announcement>> GetByTeacherAsync(int teacherId);
}