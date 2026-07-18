// File: EnrollmentSystem.BLL/Services/Implementations/AdminService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IApplicationRepository _applicationRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ISectionRepository _sectionRepo;
    private readonly ITeacherRepository _teacherRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUserRepository _userRepo;

    public AdminService(
        IStudentRepository studentRepo,
        IApplicationRepository applicationRepo,
        IEnrollmentRepository enrollmentRepo,
        ISectionRepository sectionRepo,
        ITeacherRepository teacherRepo,
        IEmployeeRepository employeeRepo,
        IUserRepository userRepo)
    {
        _studentRepo = studentRepo;
        _applicationRepo = applicationRepo;
        _enrollmentRepo = enrollmentRepo;
        _sectionRepo = sectionRepo;
        _teacherRepo = teacherRepo;
        _employeeRepo = employeeRepo;
        _userRepo = userRepo;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalStudents = await _studentRepo.CountAsync(s => !s.IsDeleted),
            TotalApplications = await _applicationRepo.CountAsync(a => !a.IsDeleted),
            PendingApplications = await _applicationRepo.CountByStatusAsync("Pending"),
            PendingEnrollments = await _enrollmentRepo.CountAsync(e => !e.IsDeleted && e.Status == "Pending"),
            TotalSections = await _sectionRepo.CountAsync(s => !s.IsDeleted),
            TotalTeachers = await _teacherRepo.CountAsync(t => !t.IsDeleted)
        };
    }

    public async Task<IEnumerable<AspNetUser>> GetUsersAsync()
        => await _userRepo.GetAllAsync();

    public async Task<ServiceResult<int>> CreateEmployeeAsync(Employee employee, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(employee.FirstName) || string.IsNullOrWhiteSpace(employee.LastName))
            return ServiceResult<int>.Fail("Employee first and last name are required.");

        employee.CreatedBy = createdBy;
        employee.IsDeleted = false;

        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveAsync();

        return ServiceResult<int>.Ok(employee.EmployeeId, "Employee created.");
    }

    public async Task<ServiceResult<int>> PromoteToTeacherAsync(int employeeId, string createdBy)
    {
        var existing = await _teacherRepo.GetByEmployeeIdAsync(employeeId);
        if (existing is not null)
            return ServiceResult<int>.Fail("This employee is already a teacher.");

        var teacher = new Teacher
        {
            EmployeeId = employeeId,
            CreatedBy = createdBy,
            IsDeleted = false
        };

        await _teacherRepo.AddAsync(teacher);
        await _teacherRepo.SaveAsync();

        return ServiceResult<int>.Ok(teacher.TeacherId, "Employee promoted to teacher.");
    }
}