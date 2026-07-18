// File: EnrollmentSystem.DAL/Repositories/Interfaces/ITeacherRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface ITeacherRepository : IGenericRepository<Teacher>
{
    Task<IEnumerable<Teacher>> GetAllWithEmployeeAsync();
    Task<Teacher?> GetWithEmployeeAsync(int teacherId);
    Task<Teacher?> GetByEmployeeIdAsync(int employeeId);
}