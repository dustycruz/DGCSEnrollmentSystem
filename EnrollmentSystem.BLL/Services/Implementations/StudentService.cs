// File: EnrollmentSystem.BLL/Services/Implementations/StudentService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IScheduleRepository _scheduleRepo;

    public StudentService(
        IStudentRepository studentRepo,
        IEnrollmentRepository enrollmentRepo,
        IScheduleRepository scheduleRepo)
    {
        _studentRepo = studentRepo;
        _enrollmentRepo = enrollmentRepo;
        _scheduleRepo = scheduleRepo;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
        => await _studentRepo.GetAllActiveAsync();

    public async Task<Student?> GetProfileAsync(int studentId)
        => await _studentRepo.GetFullProfileAsync(studentId);

    public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
        => await _studentRepo.GetByStudentNumberAsync(studentNumber);

    public async Task<ServiceResult> UpdateProfileAsync(Student student, string modifiedBy)
    {
        var existing = await _studentRepo.GetByIdAsync(student.StudentId);
        if (existing is null)
            return ServiceResult.Fail("Student not found.");

        existing.FirstName = student.FirstName;
        existing.MiddleName = student.MiddleName;
        existing.LastName = student.LastName;
        existing.Address = student.Address;
        existing.MobileNumber = student.MobileNumber;
        existing.EmailAddress = student.EmailAddress;
        existing.HomeNumber = student.HomeNumber;
        existing.GuardianName = student.GuardianName;
        existing.GuardianRelationship = student.GuardianRelationship;
        existing.GuardianEmailAddress = student.GuardianEmailAddress;
        existing.ModifiedBy = modifiedBy;

        _studentRepo.Update(existing);
        await _studentRepo.SaveAsync();

        return ServiceResult.Ok("Profile updated.");
    }

    public async Task<ServiceResult<int>> CreateFromApplicationAsync(Application application, string createdBy)
    {
        var student = new Student
        {
            FirstName = application.FirstName,
            MiddleName = application.MiddleName,
            LastName = application.LastName,
            Birthday = application.Birthday,
            Gender = application.Gender,
            Address = application.Address,
            MobileNumber = application.MobileNumber,
            LearnerReferenceNumber = application.LearnerReferenceNumber,
            PlaceOfBirth = application.PlaceOfBirth,
            Religion = application.Religion,
            EmailAddress = application.EmailAddress,
            HomeNumber = application.HomeNumber,
            GuardianName = application.GuardianName,
            ApplicationId = application.ApplicationId,
            StudentNumber = await _studentRepo.GenerateStudentNumberAsync(),
            CreatedBy = createdBy,
            IsDeleted = false
        };

        if (application.Birthday.HasValue)
            student.Age = CalculateAge(application.Birthday.Value);

        await _studentRepo.AddAsync(student);
        await _studentRepo.SaveAsync();

        return ServiceResult<int>.Ok(student.StudentId, $"Student record created ({student.StudentNumber}).");
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsAsync(int studentId)
        => await _enrollmentRepo.GetByStudentAsync(studentId);

    public async Task<IEnumerable<Schedule>> GetScheduleAsync(int studentId)
    {
        var enrollments = (await _enrollmentRepo.GetByStudentAsync(studentId)).ToList();
        var active = enrollments.FirstOrDefault(e => e.Status == "Enrolled") ?? enrollments.FirstOrDefault();
        if (active is null)
            return Enumerable.Empty<Schedule>();

        return await _scheduleRepo.GetBySectionAsync(active.SectionId);
    }

    private static int CalculateAge(DateOnly birthday)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthday.Year;
        if (birthday > today.AddYears(-age)) age--;
        return age;
    }
}