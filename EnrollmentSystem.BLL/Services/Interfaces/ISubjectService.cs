// File: EnrollmentSystem.BLL/Services/Interfaces/ISubjectService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface ISubjectService
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject?> GetAsync(int id);
    Task<ServiceResult<int>> CreateAsync(Subject subject, string createdBy);
    Task<ServiceResult> UpdateAsync(Subject subject, string modifiedBy);
    Task<ServiceResult> DeleteAsync(int id, string modifiedBy);
}