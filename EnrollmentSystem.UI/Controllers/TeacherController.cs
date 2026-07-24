// File: EnrollmentSystem.UI/Controllers/TeacherController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly IGradeService _gradeService;
    private readonly IAnnouncementService _announcementService;

    public TeacherController(
        ITeacherService teacherService,
        IGradeService gradeService,
        IAnnouncementService announcementService)
    {
        _teacherService = teacherService;
        _gradeService = gradeService;
        _announcementService = announcementService;
    }

    private async Task<Teacher?> MeAsync()
        => await _teacherService.GetByEmployeeNumberAsync(User.GetUserName());

    public async Task<IActionResult> Index()
    {
        var teacher = await MeAsync();
        var vm = new TeacherDashboardViewModel { Teacher = teacher };
        if (teacher is null) return View(vm);

        vm.TeacherName = teacher.Employee is null ? "Teacher" : $"{teacher.Employee.FirstName} {teacher.Employee.LastName}";

        // The class list is the essential content; the widgets below are best-effort so a
        // single bad row can never take the whole dashboard down.
        List<Schedule> schedules;
        try
        {
            schedules = (await _teacherService.GetScheduleAsync(teacher.TeacherId)).ToList();
        }
        catch
        {
            schedules = new List<Schedule>();
        }
        vm.Schedules = schedules;
        vm.ClassCount = schedules.Count;

        try
        {
            var todayName = DateTime.Now.DayOfWeek.ToString();
            vm.Today = todayName;
            foreach (var s in schedules)
            {
                var details = s.ScheduleDetails ?? new List<ScheduleDetail>();
                foreach (var d in details.Where(x => !x.IsDeleted &&
                             string.Equals(x.Day, todayName, StringComparison.OrdinalIgnoreCase)))
                {
                    var time = (d.StartTime.HasValue && d.EndTime.HasValue)
                        ? $"{d.StartTime.Value.ToString("h:mm tt")} - {d.EndTime.Value.ToString("h:mm tt")}"
                        : "-";
                    vm.TodayClasses.Add(new TodayClassItem
                    {
                        ScheduleId = s.ScheduleId,
                        Subject = s.Subject?.Name ?? "-",
                        Section = s.Section?.Name ?? "-",
                        Room = d.Room?.Name ?? "-",
                        Start = d.StartTime,
                        Time = time
                    });
                }
            }
            vm.TodayClasses = vm.TodayClasses.OrderBy(t => t.Start ?? TimeOnly.MaxValue).ToList();

            var advisory = (await _teacherService.GetAdvisoryAsync(teacher.TeacherId)).ToList();
            vm.AdvisoryClassName = advisory.FirstOrDefault()?.Name;
            foreach (var sec in advisory)
            {
                var roster = await _teacherService.GetClassListAsync(sec.SectionId);
                vm.AdviseeCount += roster.Count(e => e.Status == "Enrolled");
            }

            var quarters = new[] { "Q1", "Q2", "Q3", "Q4" };
            foreach (var s in schedules)
            {
                foreach (var q in quarters)
                {
                    var grades = await _gradeService.GetBySubjectAndQuarterAsync(s.SubjectId, q);
                    if (!grades.Any()) vm.PendingGradeQuarters++;
                }
            }
        }
        catch
        {
            // Widgets are optional — never let them break the page.
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
    public async Task<IActionResult> Advisory()
    {
        var teacher = await MeAsync();
        if (teacher is null) return RedirectToAction(nameof(Index));
        return View(await _teacherService.GetAdvisoryAsync(teacher.TeacherId));
    }
    // Teacher's own classes (taught + advisory), de-duplicated, for the post target list.
    private async Task<List<Section>> MyClassesAsync(int teacherId)
    {
        var taught = await _teacherService.GetSectionsAsync(teacherId);
        var advisory = await _teacherService.GetAdvisoryAsync(teacherId);
        return taught.Concat(advisory)
            .GroupBy(s => s.SectionId)
            .Select(g => g.First())
            .OrderBy(s => s.Name)
            .ToList();
    }

    public async Task<IActionResult> Announcements()
    {
        var teacher = await MeAsync();
        var vm = new TeacherAnnouncementsViewModel();
        if (teacher is null) return View(vm);

        var classes = await MyClassesAsync(teacher.TeacherId);
        vm.Sections = classes.Select(s => new SelectListItem(s.Name, s.SectionId.ToString()));
        vm.SectionNames = classes.ToDictionary(s => s.SectionId, s => s.Name);
        vm.Posts = (await _announcementService.GetByTeacherAsync(teacher.TeacherId)).ToList();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostAnnouncement(int? sectionId, string title, string content)
    {
        var teacher = await MeAsync();
        if (teacher is null) return RedirectToAction(nameof(Index));

        if (sectionId.HasValue)
        {
            var classes = await MyClassesAsync(teacher.TeacherId);
            if (classes.All(c => c.SectionId != sectionId.Value))
            {
                TempData["Error"] = "You can only post to your own classes.";
                return RedirectToAction(nameof(Announcements));
            }
        }

        var result = await _announcementService.CreateAsync(new Announcement
        {
            Title = title,
            Content = content,
            SectionId = sectionId,
            TeacherId = teacher.TeacherId
        }, User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Announcements));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var result = await _announcementService.DeleteAsync(id, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Announcements));
    }
}