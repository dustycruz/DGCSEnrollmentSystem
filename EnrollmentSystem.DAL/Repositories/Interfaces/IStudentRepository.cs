// File: EnrollmentSystem.DAL/Repositories/Interfaces/IStudentRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<IEnumerable<Student>> GetAllActiveAsync();
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
    Task<Student?> GetFullProfileAsync(int studentId);
    Task<IEnumerable<Student>> SearchAsync(string term);
    Task<string> GenerateStudentNumberAsync();
}