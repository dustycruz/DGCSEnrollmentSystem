// File: EnrollmentSystem.UI/Controllers/EnrollmentController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class EnrollmentController : Controller
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ISectionService _sectionService;
    private readonly IStudentService _studentService;

    public EnrollmentController(
        IEnrollmentService enrollmentService,
        ISectionService sectionService,
        IStudentService studentService)
    {
        _enrollmentService = enrollmentService;
        _sectionService = sectionService;
        _studentService = studentService;
    }

    // Sidebar "Enrollment" lands here: pick a section to manage.
    public async Task<IActionResult> Index()
        => View(await _sectionService.GetAllAsync());

    public async Task<IActionResult> Manage(int sectionId)
    {
        var section = await _sectionService.GetAsync(sectionId);
        if (section is null) return NotFound();

        var enrolled = (await _enrollmentService.GetBySectionAsync(sectionId)).ToList();
        var enrolledIds = enrolled.Select(e => e.StudentId).ToHashSet();
        var available = (await _studentService.GetAllAsync()).Where(s => !enrolledIds.Contains(s.StudentId));

        return View(new EnrollmentManageViewModel
        {
            SectionId = sectionId,
            SectionName = section.Name,
            Enrollments = enrolled,
            Students = available.Select(s => new SelectListItem(
                $"{s.LastName}, {s.FirstName} ({s.StudentNumber})", s.StudentId.ToString()))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int sectionId, int studentId)
    {
        // Create enrollment then approve it — approval auto-generates the student's class schedule.
        var enroll = await _enrollmentService.EnrollStudentAsync(studentId, sectionId, User.GetUserName());
        if (enroll.Success)
        {
            await _enrollmentService.ApproveAsync(enroll.Data, User.GetUserName());
            TempData["Success"] = "Student enrolled and schedule generated.";
        }
        else
        {
            TempData["Error"] = enroll.Message;
        }
        return RedirectToAction(nameof(Manage), new { sectionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int enrollmentId, int sectionId)
    {
        var result = await _enrollmentService.RejectAsync(enrollmentId, "Removed by admin", User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Manage), new { sectionId });
    }
}