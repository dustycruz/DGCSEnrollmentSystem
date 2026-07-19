// File: EnrollmentSystem.UI/Controllers/StudentController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IReportingService _reporting;
    private readonly IAnnouncementService _announcementService;

    public StudentController(
        IStudentService studentService,
        IReportingService reporting,
        IAnnouncementService announcementService)
    {
        _studentService = studentService;
        _reporting = reporting;
        _announcementService = announcementService;
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
        var schedules = await _studentService.GetScheduleAsync(s.StudentId);
        return View(schedules);
    }

    public async Task<IActionResult> Grades()
    {
        var s = await MeAsync();
        if (s is null) return RedirectToAction(nameof(Index));
        var reportCard = await _reporting.GetReportCardAsync(s.StudentId);
        return View(reportCard);
    }

    public async Task<IActionResult> Announcements()
    {
        var items = await _announcementService.GetRecentAsync(50);
        return View(items);
    }
}