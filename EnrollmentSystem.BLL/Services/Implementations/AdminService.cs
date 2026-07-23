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
    private readonly IGradeLevelRepository _gradeLevelRepo;
    private readonly ISchoolYearRepository _schoolYearRepo;
    private readonly IAuditService _audit;

    public AdminService(
        IStudentRepository studentRepo,
        IApplicationRepository applicationRepo,
        IEnrollmentRepository enrollmentRepo,
        ISectionRepository sectionRepo,
        ITeacherRepository teacherRepo,
        IEmployeeRepository employeeRepo,
        IUserRepository userRepo,
        IGradeLevelRepository gradeLevelRepo,
        ISchoolYearRepository schoolYearRepo,
        IAuditService audit)
    {
        _studentRepo = studentRepo;
        _applicationRepo = applicationRepo;
        _enrollmentRepo = enrollmentRepo;
        _sectionRepo = sectionRepo;
        _teacherRepo = teacherRepo;
        _employeeRepo = employeeRepo;
        _userRepo = userRepo;
        _gradeLevelRepo = gradeLevelRepo;
        _schoolYearRepo = schoolYearRepo;
        _audit = audit;
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

    public async Task EnsureDemoTeacherAsync()
    {
        const string empNo = "EMP-2026-0001";
        if (await _userRepo.UserNameExistsAsync(empNo)) return;

        var employee = new Employee
        {
            FirstName = "Maria",
            MiddleName = "L",
            LastName = "Santos",
            EmployeeNumber = empNo,
            EmailAddress = "maria.santos@dgcs.edu.ph",
            CreatedBy = "system",
            IsDeleted = false
        };
        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveAsync();

        var teacher = new Teacher { EmployeeId = employee.EmployeeId, CreatedBy = "system", IsDeleted = false };
        await _teacherRepo.AddAsync(teacher);
        await _teacherRepo.SaveAsync();

        var hash = PasswordHasher.Hash("Teacher@123");
        var user = new AspNetUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = empNo,
            Email = employee.EmailAddress,
            PasswordHash = hash,
            EmailConfirmed = true,
            MustChangePassword = false
        };
        var role = await _userRepo.EnsureRoleAsync("Teacher");
        await _userRepo.AddUserAsync(user);
        await _userRepo.AddUserToRoleAsync(user.Id, role.Id, empNo, hash);
        await _userRepo.SaveAsync();
    }

    public async Task<ServiceResult<AccountCredentialsDto>> CreateTeacherAsync(Employee employee, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(employee.FirstName) || string.IsNullOrWhiteSpace(employee.LastName))
            return ServiceResult<AccountCredentialsDto>.Fail("First and last name are required.");

        // Employee number doubles as the login username; auto-generate if blank.
        var empNo = employee.EmployeeNumber?.Trim();
        if (string.IsNullOrWhiteSpace(empNo))
        {
            var year = DateTime.Now.Year;
            var n = await _employeeRepo.CountAsync() + 1;
            empNo = $"EMP-{year}-{n:D4}";
            while (await _userRepo.UserNameExistsAsync(empNo)) { n++; empNo = $"EMP-{year}-{n:D4}"; }
        }
        else if (await _userRepo.UserNameExistsAsync(empNo))
        {
            return ServiceResult<AccountCredentialsDto>.Fail("That employee number is already in use.");
        }

        employee.EmployeeNumber = empNo;
        employee.CreatedBy = createdBy;
        employee.IsDeleted = false;
        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveAsync();

        var teacher = new Teacher { EmployeeId = employee.EmployeeId, CreatedBy = createdBy, IsDeleted = false };
        await _teacherRepo.AddAsync(teacher);
        await _teacherRepo.SaveAsync();

        var temp = CredentialGenerator.GenerateTempPassword();
        var hash = PasswordHasher.Hash(temp);
        var user = new AspNetUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = empNo,
            Email = employee.EmailAddress,
            PasswordHash = hash,
            EmailConfirmed = true,
            MustChangePassword = true
        };
        var role = await _userRepo.EnsureRoleAsync("Teacher");
        await _userRepo.AddUserAsync(user);
        await _userRepo.AddUserToRoleAsync(user.Id, role.Id, empNo, hash);
        await _userRepo.SaveAsync();

        await _audit.LogAsync(
            action: "Teacher Created",
            entityName: "Teacher",
            entityId: teacher.TeacherId.ToString(),
            description: $"Teacher {employee.FirstName} {employee.LastName} ({empNo}) created by {createdBy}.",
            status: "Created");

        return ServiceResult<AccountCredentialsDto>.Ok(
            new AccountCredentialsDto { UserName = empNo, TempPassword = temp },
            $"Teacher account created ({empNo}).");
    }

    public async Task<ServiceResult> UpdateTeacherAsync(int teacherId, Employee updated, string modifiedBy)
    {
        var teacher = await _teacherRepo.GetWithEmployeeAsync(teacherId);
        if (teacher?.Employee is null) return ServiceResult.Fail("Teacher not found.");

        var emp = teacher.Employee;
        emp.FirstName = updated.FirstName;
        emp.MiddleName = updated.MiddleName;
        emp.LastName = updated.LastName;
        emp.EmailAddress = updated.EmailAddress;
        emp.ModifiedBy = modifiedBy;
        _employeeRepo.Update(emp);
        await _employeeRepo.SaveAsync();

        // keep the login email in sync
        if (!string.IsNullOrWhiteSpace(emp.EmployeeNumber))
        {
            var user = await _userRepo.GetByUsernameAsync(emp.EmployeeNumber);
            if (user is not null)
            {
                user.Email = emp.EmailAddress;
                await _userRepo.SaveAsync();
            }
        }

        await _audit.LogAsync(
            action: "Teacher Updated",
            entityName: "Teacher",
            entityId: teacherId.ToString(),
            description: $"Teacher {emp.FirstName} {emp.LastName} ({emp.EmployeeNumber}) updated by {modifiedBy}.",
            status: "Updated");

        return ServiceResult.Ok("Teacher updated.");
    }

    public async Task<ServiceResult> DeleteTeacherAsync(int teacherId, string modifiedBy)
    {
        var teacher = await _teacherRepo.GetWithEmployeeAsync(teacherId);
        if (teacher is null) return ServiceResult.Fail("Teacher not found.");

        teacher.IsDeleted = true;
        teacher.ModifiedBy = modifiedBy;
        _teacherRepo.Update(teacher);

        if (teacher.Employee is not null)
        {
            teacher.Employee.IsDeleted = true;
            teacher.Employee.ModifiedBy = modifiedBy;
            _employeeRepo.Update(teacher.Employee);
        }
        await _teacherRepo.SaveAsync();

        // remove the login so they can no longer sign in (grades stay via the soft-deleted Teacher record)
        if (!string.IsNullOrWhiteSpace(teacher.Employee?.EmployeeNumber))
            await _userRepo.DeleteUserByUserNameAsync(teacher.Employee.EmployeeNumber);

        await _audit.LogAsync(
            action: "Teacher Removed",
            entityName: "Teacher",
            entityId: teacherId.ToString(),
            description: $"Teacher {teacher.Employee?.FirstName} {teacher.Employee?.LastName} ({teacher.Employee?.EmployeeNumber}) removed by {modifiedBy}; login disabled.",
            status: "Deleted");

        return ServiceResult.Ok("Teacher removed and login disabled.");
    }

    public async Task EnsureGradeSectionsAsync()
    {
        var sy = await _schoolYearRepo.GetLatestAsync();
        if (sy is null) return;

        // One class per grade level, excluding Senior High.
        var grades = (await _gradeLevelRepo.GetAllActiveAsync())
            .Where(g => g.EducationalLevel?.Name != "Senior High School")
            .ToList();

        var added = false;
        foreach (var g in grades)
        {
            var exists = await _sectionRepo.ExistsAsync(s =>
                !s.IsDeleted && s.GradeLevelId == g.GradeLevelId && s.SchoolYearId == sy.SchoolYearId);

            if (!exists)
            {
                await _sectionRepo.AddAsync(new Section
                {
                    Name = g.Name,          // the class is named after its grade (e.g., "Grade 10")
                    GradeLevelId = g.GradeLevelId,
                    SchoolYearId = sy.SchoolYearId,
                    CreatedBy = "system",
                    IsDeleted = false
                });
                added = true;
            }
        }
        if (added) await _sectionRepo.SaveAsync();
    }
}