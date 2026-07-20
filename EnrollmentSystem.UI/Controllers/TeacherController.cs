// File: EnrollmentSystem.UI/Controllers/TeacherController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly IGradeService _gradeService;

    public TeacherController(ITeacherService teacherService, IGradeService gradeService)
    {
        _teacherService = teacherService;
        _gradeService = gradeService;
    }

    private async Task<Teacher?> MeAsync()
        => await _teacherService.GetByEmployeeNumberAsync(User.GetUserName());

    public async Task<IActionResult> Index()
    {
        var teacher = await MeAsync();
        var vm = new TeacherDashboardViewModel { Teacher = teacher };
        if (teacher is not null)
        {
            vm.TeacherName = teacher.Employee is null ? "Teacher" : $"{teacher.Employee.FirstName} {teacher.Employee.LastName}";
            vm.Schedules = await _teacherService.GetScheduleAsync(teacher.TeacherId);
        }
        return View(vm);
    }

    public async Task<IActionResult> ClassList(int sectionId)
    {
        var enrollments = await _teacherService.GetClassListAsync(sectionId);
        ViewData["SectionId"] = sectionId;
        return View(enrollments);
    }

    [HttpGet]
    public async Task<IActionResult> EncodeGrades(int scheduleId, string quarter = "Q1")
    {
        var schedule = await _teacherService.GetClassAsync(scheduleId);
        if (schedule is null) return NotFound();

        var classList = await _teacherService.GetClassListAsync(schedule.SectionId);
        var existing = (await _gradeService.GetBySubjectAndQuarterAsync(schedule.SubjectId, quarter)).ToList();

        var vm = new GradeEncodeViewModel
        {
            ScheduleId = schedule.ScheduleId,
            SectionId = schedule.SectionId,
            SubjectId = schedule.SubjectId,
            GradeLevelId = schedule.Section?.GradeLevelId,
            TeacherId = schedule.TeacherId ?? 0,
            SectionName = schedule.Section?.Name ?? "-",
            SubjectName = schedule.Subject?.Name ?? "-",
            Quarter = quarter,
            Rows = classList
                .Where(e => e.Status == "Enrolled")
                .Select(e => new GradeEntryRow
                {
                    StudentId = e.StudentId,
                    StudentName = $"{e.Student.LastName}, {e.Student.FirstName}",
                    Grade = existing.FirstOrDefault(g => g.StudentId == e.StudentId)?.Grade1
                })
                .OrderBy(r => r.StudentName)
                .ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EncodeGrades(GradeEncodeViewModel vm)
    {
        var saved = 0;
        foreach (var row in vm.Rows)
        {
            if (row.Grade is null) continue;

            var result = await _gradeService.EncodeGradeAsync(new Grade
            {
                StudentId = row.StudentId,
                SubjectId = vm.SubjectId,
                Quarter = vm.Quarter,
                Grade1 = row.Grade,
                GradeLevelId = vm.GradeLevelId,
                TeacherId = vm.TeacherId == 0 ? null : vm.TeacherId
            }, User.GetUserName());

            if (result.Success) saved++;
        }

        TempData["Success"] = $"{saved} grade(s) saved for {vm.Quarter}.";
        return RedirectToAction(nameof(EncodeGrades), new { scheduleId = vm.ScheduleId, quarter = vm.Quarter });
    }
    public async Task<IActionResult> Schedule()
    {
        var teacher = await MeAsync();
        if (teacher is null) return RedirectToAction(nameof(Index));
        return View(await _teacherService.GetScheduleAsync(teacher.TeacherId));
    }
    public async Task<IActionResult> Calendar(int? year, int? month)
    {
        var teacher = await MeAsync();
        if (teacher is null) return RedirectToAction(nameof(Index));
        var schedules = await _teacherService.GetScheduleAsync(teacher.TeacherId);
        return View(EnrollmentSystem.UI.Helpers.CalendarBuilder.FromSchedules(schedules, year, month, "Teaching Calendar"));
    }
}