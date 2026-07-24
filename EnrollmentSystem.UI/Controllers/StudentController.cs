// File: EnrollmentSystem.UI/Controllers/StudentController.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IReportingService _reporting;
    private readonly IAnnouncementService _announcementService;
    private readonly IProofOfPaymentService _proofService;
    private readonly IFileStorageService _fileStorage;
    private readonly PaymentInstructionSettings _paymentInfo;

    public StudentController(
        IStudentService studentService,
        IReportingService reporting,
        IAnnouncementService announcementService,
        IProofOfPaymentService proofService,
        IFileStorageService fileStorage,
        IOptions<PaymentInstructionSettings> paymentInfo)
    {
        _studentService = studentService;
        _reporting = reporting;
        _announcementService = announcementService;
        _proofService = proofService;
        _fileStorage = fileStorage;
        _paymentInfo = paymentInfo.Value;
    }

    private async Task<Student?> MeAsync()
        => await _studentService.GetByStudentNumberAsync(User.GetUserName());

    public async Task<IActionResult> Index()
    {
        var student = await MeAsync();
        var vm = new StudentDashboardViewModel { Student = student };
        if (student is not null)
        {
            vm.Enrollments = await _studentService.GetEnrollmentsAsync(student.StudentId);
            vm.Announcements = await _announcementService.GetRecentAsync(5);
        }
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        return View(new StudentProfileViewModel
        {
            StudentId = s.StudentId,
            StudentNumber = s.StudentNumber ?? "-",
            FirstName = s.FirstName,
            MiddleName = s.MiddleName,
            LastName = s.LastName,
            Address = s.Address,
            MobileNumber = s.MobileNumber,
            EmailAddress = s.EmailAddress,
            HomeNumber = s.HomeNumber,
            GuardianName = s.GuardianName,
            GuardianRelationship = s.GuardianRelationship,
            GuardianEmailAddress = s.GuardianEmailAddress
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(StudentProfileViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _studentService.UpdateProfileAsync(new Student
        {
            StudentId = vm.StudentId,
            FirstName = vm.FirstName,
            MiddleName = vm.MiddleName,
            LastName = vm.LastName,
            Address = vm.Address,
            MobileNumber = vm.MobileNumber,
            EmailAddress = vm.EmailAddress,
            HomeNumber = vm.HomeNumber,
            GuardianName = vm.GuardianName,
            GuardianRelationship = vm.GuardianRelationship,
            GuardianEmailAddress = vm.GuardianEmailAddress
        }, User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> Schedule()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        return View(await _studentService.GetScheduleAsync(s.StudentId));
    }

    public async Task<IActionResult> Subjects()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        return View(await _studentService.GetScheduleAsync(s.StudentId));
    }
    // Details for one of the student's own subjects (opened by clicking a subject card).
    public async Task<IActionResult> SubjectDetails(int scheduleId)
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));

        // Restrict to the student's own schedule so they can't view arbitrary subjects.
        var schedules = (await _studentService.GetScheduleAsync(s.StudentId)).ToList();
        var schedule = schedules.FirstOrDefault(x => x.ScheduleId == scheduleId);
        if (schedule is null) return RedirectToAction(nameof(Subjects));

        var card = await _reporting.GetReportCardAsync(s.StudentId);
        ViewBag.Grades = card?.Lines.FirstOrDefault(l => l.Subject == (schedule.Subject?.Name ?? ""));

        return View(schedule);
    }

    public async Task<IActionResult> Calendar(int? year, int? month)
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        var schedules = await _studentService.GetScheduleAsync(s.StudentId);
        return View(EnrollmentSystem.UI.Helpers.CalendarBuilder.FromSchedules(schedules, year, month, "My Calendar"));
    }

    public async Task<IActionResult> Grades()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        return View(await _reporting.GetReportCardAsync(s.StudentId));
    }

    public async Task<IActionResult> Announcements()
    {
        // Show school-wide announcements plus the student's own class announcements.
        var student = await MeAsync();
        int? sectionId = null;
        if (student is not null)
        {
            var enrollments = (await _studentService.GetEnrollmentsAsync(student.StudentId)).ToList();
            var active = enrollments.FirstOrDefault(e => e.Status == "Enrolled") ?? enrollments.FirstOrDefault();
            sectionId = active?.SectionId;
        }
        return View(await _announcementService.GetFeedForSectionAsync(sectionId));
    }

    public async Task<IActionResult> Payments()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));

        var payments = (await _proofService.GetByStudentAsync(s.StudentId)).ToList();
        decimal.TryParse(_paymentInfo.EnrollmentFee, out var fee);
        var paid = payments.Where(p => p.Status == "Verified" && p.AmountPaid.HasValue).Sum(p => p.AmountPaid!.Value);

        return View(new StudentPaymentsViewModel
        {
            Info = _paymentInfo,
            Payments = payments,
            TotalFee = fee,
            Paid = paid,
            Balance = Math.Max(0, fee - paid)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPayment(string referenceNumber, decimal? amountPaid, string? paymentMethod, IFormFile file)
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));

        var saved = await _fileStorage.SaveFileAsync(file, "payments");
        if (!saved.Success)
        {
            TempData["Error"] = saved.Message;
            return RedirectToAction(nameof(Payments));
        }

        await _proofService.SubmitAsync(new ProofOfPayment
        {
            StudentId = s.StudentId,
            ReferenceNumber = referenceNumber,
            AmountPaid = amountPaid,
            PaymentMethod = paymentMethod,
            Purpose = PaymentPurposes.EnrollmentFee,
            FilePath = saved.Data,
            PaymentDate = DateTime.Now
        }, User.GetUserName());

        TempData["Success"] = "Proof of payment submitted for verification.";
        return RedirectToAction(nameof(Payments));
    }
}