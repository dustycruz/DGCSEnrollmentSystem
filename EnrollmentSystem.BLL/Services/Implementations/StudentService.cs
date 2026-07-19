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
    private readonly IApplicationRepository _applicationRepo;
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public StudentService(
        IStudentRepository studentRepo,
        IEnrollmentRepository enrollmentRepo,
        IScheduleRepository scheduleRepo,
        IApplicationRepository applicationRepo,
        IUserRepository userRepo,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _studentRepo = studentRepo;
        _enrollmentRepo = enrollmentRepo;
        _scheduleRepo = scheduleRepo;
        _applicationRepo = applicationRepo;
        _userRepo = userRepo;
        _emailService = emailService;
        _notificationService = notificationService;
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

    public async Task<ServiceResult<int>> FinalizeEnrollmentAsync(int applicationId, string modifiedBy)
    {
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application is null)
            return ServiceResult<int>.Fail("Application not found.");

        if (application.Status == ApplicationStatuses.Enrolled)
            return ServiceResult<int>.Fail("This application is already enrolled.");

        if (application.Status != ApplicationStatuses.Approved)
            return ServiceResult<int>.Fail("Only approved applications can be finalized.");

        if (string.IsNullOrWhiteSpace(application.EmailAddress))
            return ServiceResult<int>.Fail("The application has no email address on file.");

        // 1. Create the Student record
        var created = await CreateFromApplicationAsync(application, modifiedBy);
        if (!created.Success)
            return created;

        var student = await _studentRepo.GetByIdAsync(created.Data);
        var studentNumber = student!.StudentNumber!;

        // 2. Create the student login: UserName = Student Number, temp password, forced change
        var tempPassword = CredentialGenerator.GenerateTempPassword();
        var hash = PasswordHasher.Hash(tempPassword);
        var user = new AspNetUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = studentNumber,
            Email = application.EmailAddress,
            PasswordHash = hash,
            EmailConfirmed = true,
            MustChangePassword = true
        };
        var role = await _userRepo.EnsureRoleAsync("Student");
        await _userRepo.AddUserAsync(user);
        await _userRepo.AddUserToRoleAsync(user.Id, role.Id, studentNumber, hash);
        await _userRepo.SaveAsync();

        // 3. Mark the application Enrolled
        application.Status = ApplicationStatuses.Enrolled;
        application.ModifiedBy = modifiedBy;
        _applicationRepo.Update(application);
        await _applicationRepo.SaveAsync();

        // 4. Notify: email credentials + portal notification to the applicant account
        await _emailService.SendStudentCredentialsAsync(application.EmailAddress, studentNumber, tempPassword);

        var applicantUser = string.IsNullOrWhiteSpace(application.CreatedBy)
            ? null
            : await _userRepo.GetByUsernameAsync(application.CreatedBy);
        if (applicantUser is not null)
        {
            await _notificationService.NotifyAsync(applicantUser.Id,
                "You are officially enrolled!",
                $"Your Student Number is {studentNumber}. Check your email for your Student Portal credentials.");
        }

        return ServiceResult<int>.Ok(created.Data,
            $"Enrollment finalized. Student Number {studentNumber} created and credentials emailed.");
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