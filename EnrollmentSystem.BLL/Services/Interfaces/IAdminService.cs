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
}