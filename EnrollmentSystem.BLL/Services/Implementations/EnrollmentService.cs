// File: EnrollmentSystem.BLL/Services/Implementations/EnrollmentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IScheduleRepository _scheduleRepo;
    private readonly IGenericRepository<StudentSchedule> _studentScheduleRepo;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepo,
        IScheduleRepository scheduleRepo,
        IGenericRepository<StudentSchedule> studentScheduleRepo)
    {
        _enrollmentRepo = enrollmentRepo;
        _scheduleRepo = scheduleRepo;
        _studentScheduleRepo = studentScheduleRepo;
    }

    public async Task<ServiceResult<int>> EnrollStudentAsync(int studentId, int sectionId, string createdBy)
    {
        if (await _enrollmentRepo.IsStudentEnrolledAsync(studentId, sectionId))
            return ServiceResult<int>.Fail("Student is already enrolled in this section.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            SectionId = sectionId,
            Status = "Pending",
            CreatedBy = createdBy,
            IsDeleted = false
        };

        await _enrollmentRepo.AddAsync(enrollment);
        await _enrollmentRepo.SaveAsync();

        return ServiceResult<int>.Ok(enrollment.EnrollmentId, "Enrollment request submitted.");
    }

    public async Task<IEnumerable<Enrollment>> GetPendingAsync()
        => await _enrollmentRepo.GetByStatusAsync("Pending");

    public async Task<IEnumerable<Enrollment>> GetBySectionAsync(int sectionId)
        => await _enrollmentRepo.GetBySectionAsync(sectionId);

    public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        => await _enrollmentRepo.GetByStudentAsync(studentId);

    public async Task<Enrollment?> GetAsync(int id)
        => await _enrollmentRepo.GetFullAsync(id);

    public async Task<ServiceResult> ApproveAsync(int enrollmentId, string modifiedBy)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(enrollmentId);
        if (enrollment is null)
            return ServiceResult.Fail("Enrollment not found.");

        enrollment.Status = "Enrolled";
        enrollment.ModifiedBy = modifiedBy;
        _enrollmentRepo.Update(enrollment);

        // AUTOMATED ENROLLMENT: generate the student's class schedule from the section's schedules.
        var alreadyGenerated = await _studentScheduleRepo.FindAsync(ss => ss.EnrollmentId == enrollmentId);
        if (!alreadyGenerated.Any())
        {
            var sectionSchedules = await _scheduleRepo.GetBySectionAsync(enrollment.SectionId);
            foreach (var schedule in sectionSchedules)
            {
                await _studentScheduleRepo.AddAsync(new StudentSchedule
                {
                    EnrollmentId = enrollmentId,
                    ScheduleId = schedule.ScheduleId
                });
            }
        }

        // Shared DbContext (scoped) — one save persists both the status change and the new schedules.
        await _enrollmentRepo.SaveAsync();

        return ServiceResult.Ok("Enrollment approved and class schedule generated.");
    }

    public async Task<ServiceResult> RejectAsync(int enrollmentId, string reason, string modifiedBy)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(enrollmentId);
        if (enrollment is null)
            return ServiceResult.Fail("Enrollment not found.");

        enrollment.Status = "Rejected";
        enrollment.ModifiedBy = modifiedBy;
        _enrollmentRepo.Update(enrollment);
        await _enrollmentRepo.SaveAsync();

        return ServiceResult.Ok("Enrollment rejected.");
    }
}