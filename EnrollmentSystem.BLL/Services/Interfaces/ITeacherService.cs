// File: EnrollmentSystem.BLL/Services/Interfaces/ITeacherService.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAllAsync();
    Task<Teacher?> GetAsync(int teacherId);
    Task<Teacher?> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<Schedule>> GetScheduleAsync(int teacherId);
    Task<IEnumerable<Section>> GetSectionsAsync(int teacherId);
    Task<IEnumerable<Enrollment>> GetClassListAsync(int sectionId);
}