// File: EnrollmentSystem.BLL/Services/Interfaces/ICurriculumService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface ICurriculumService
{
    Task<IEnumerable<Curriculum>> GetAllAsync();
    Task<Curriculum?> GetWithSubjectsAsync(int id);
    Task<ServiceResult<int>> CreateAsync(Curriculum curriculum, string createdBy);
    Task<ServiceResult<int>> AddSubjectAsync(CurriculumSubject curriculumSubject, string createdBy);
    Task<ServiceResult> DeleteAsync(int id, string modifiedBy);
}