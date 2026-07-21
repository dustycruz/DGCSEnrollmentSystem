// File: EnrollmentSystem.DAL/Repositories/Interfaces/IScheduleRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<IEnumerable<Schedule>> GetBySectionAsync(int sectionId);
    Task<IEnumerable<Schedule>> GetByTeacherAsync(int teacherId);
    Task<Schedule?> GetFullAsync(int scheduleId);
    Task<IEnumerable<Schedule>> GetAllWithDetailsAsync();
}