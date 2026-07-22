// File: EnrollmentSystem.BLL/Services/Implementations/TeacherService.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepo;
    private readonly IScheduleRepository _scheduleRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly ISectionRepository _sectionRepo;

    public TeacherService(
        ITeacherRepository teacherRepo,
        IScheduleRepository scheduleRepo,
        IEnrollmentRepository enrollmentRepo,
        IEmployeeRepository employeeRepo,
        ISectionRepository sectionRepo)
    {
        _teacherRepo = teacherRepo;
        _scheduleRepo = scheduleRepo;
        _enrollmentRepo = enrollmentRepo;
        _employeeRepo = employeeRepo;
        _sectionRepo = sectionRepo;
    }

    public async Task<IEnumerable<Teacher>> GetAllAsync()
        => await _teacherRepo.GetAllWithEmployeeAsync();

    public async Task<Teacher?> GetAsync(int teacherId)
        => await _teacherRepo.GetWithEmployeeAsync(teacherId);

    public async Task<Teacher?> GetByEmployeeIdAsync(int employeeId)
        => await _teacherRepo.GetByEmployeeIdAsync(employeeId);

    public async Task<Teacher?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        var employee = await _employeeRepo.GetByEmployeeNumberAsync(employeeNumber);
        if (employee is null) return null;
        return await _teacherRepo.GetByEmployeeIdAsync(employee.EmployeeId);
    }

    public async Task<Schedule?> GetClassAsync(int scheduleId)
        => await _scheduleRepo.GetFullAsync(scheduleId);

    public async Task<IEnumerable<Schedule>> GetScheduleAsync(int teacherId)
        => await _scheduleRepo.GetByTeacherAsync(teacherId);

    public async Task<IEnumerable<Section>> GetSectionsAsync(int teacherId)
    {
        var schedules = await _scheduleRepo.GetByTeacherAsync(teacherId);
        return schedules
            .Where(s => s.Section is not null)
            .Select(s => s.Section!)
            .GroupBy(s => s.SectionId)
            .Select(g => g.First())
            .ToList();
    }

    public async Task<IEnumerable<Enrollment>> GetClassListAsync(int sectionId)
        => await _enrollmentRepo.GetBySectionAsync(sectionId);

    public async Task<IEnumerable<Section>> GetAdvisoryAsync(int teacherId)
        => await _sectionRepo.GetByAdviserAsync(teacherId);
}