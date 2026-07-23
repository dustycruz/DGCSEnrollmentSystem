// File: EnrollmentSystem.DAL/Repositories/Interfaces/IGradeRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IGradeRepository : IGenericRepository<Grade>
{
    Task<IEnumerable<Grade>> GetByStudentAsync(int studentId);
    Task<IEnumerable<Grade>> GetByStudentAndQuarterAsync(int studentId, string quarter);
    Task<IEnumerable<Grade>> GetByTeacherAsync(int teacherId);
    Task<IEnumerable<Grade>> GetBySubjectAsync(int subjectId);
    Task<Grade?> GetExistingAsync(int studentId, int subjectId, string quarter);
    Task<IEnumerable<Grade>> GetAllActiveAsync();
}