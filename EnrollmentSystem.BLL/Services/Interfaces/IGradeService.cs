// File: EnrollmentSystem.BLL/Services/Interfaces/IGradeService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId);
    Task<IEnumerable<Grade>> GetByTeacherAsync(int teacherId);
    Task<ServiceResult> EncodeGradeAsync(Grade grade, string createdBy);
    Task<decimal?> GetGeneralAverageAsync(int studentId);
}