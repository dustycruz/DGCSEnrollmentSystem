// File: EnrollmentSystem.DAL/Repositories/Interfaces/IEnrollmentRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IEnrollmentRepository : IGenericRepository<Enrollment>
{
    Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId);
    Task<IEnumerable<Enrollment>> GetBySectionAsync(int sectionId);
    Task<IEnumerable<Enrollment>> GetByStatusAsync(string status);
    Task<Enrollment?> GetFullAsync(int enrollmentId);
    Task<bool> IsStudentEnrolledAsync(int studentId, int sectionId);
    Task<int> CountBySectionAsync(int sectionId);
}