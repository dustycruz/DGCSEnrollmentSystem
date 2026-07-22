// File: EnrollmentSystem.BLL/Services/Interfaces/IAdminService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IAdminService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<AspNetUser>> GetUsersAsync();
    Task<ServiceResult<int>> CreateEmployeeAsync(Employee employee, string createdBy);
    Task<ServiceResult<int>> PromoteToTeacherAsync(int employeeId, string createdBy);
    Task EnsureDemoTeacherAsync();
    Task<ServiceResult<AccountCredentialsDto>> CreateTeacherAsync(Employee employee, string createdBy);
    Task<ServiceResult> UpdateTeacherAsync(int teacherId, Employee updated, string modifiedBy);
    Task<ServiceResult> DeleteTeacherAsync(int teacherId, string modifiedBy);
    Task EnsureGradeSectionsAsync();
}