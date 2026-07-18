// File: EnrollmentSystem.DAL/Repositories/Interfaces/IStudentParentRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IStudentParentRepository : IGenericRepository<StudentParent>
{
    Task<IEnumerable<StudentParent>> GetByStudentAsync(int studentId);
}