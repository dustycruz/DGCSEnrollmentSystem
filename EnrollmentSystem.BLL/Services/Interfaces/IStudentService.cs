// File: EnrollmentSystem.BLL/Services/Interfaces/IStudentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetProfileAsync(int studentId);
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
    Task<ServiceResult> UpdateProfileAsync(Student student, string modifiedBy);
    Task<ServiceResult<int>> CreateFromApplicationAsync(Application application, string createdBy);
    Task<IEnumerable<Enrollment>> GetEnrollmentsAsync(int studentId);
    Task<IEnumerable<Schedule>> GetScheduleAsync(int studentId);

    Task<ServiceResult<StudentCredentialsDto>> FinalizeEnrollmentAsync(int applicationId, string modifiedBy);
}