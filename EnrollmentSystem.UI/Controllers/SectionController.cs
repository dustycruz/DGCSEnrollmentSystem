// File: EnrollmentSystem.UI/Controllers/SectionController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class SectionController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly ILookupService _lookup;

    public SectionController(ISectionService sectionService, ILookupService lookup)
    {
        _sectionService = sectionService;
        _lookup = lookup;
    }

    public async Task<IActionResult> Index()
        => View(await _sectionService.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
        => View(await BuildCreateVmAsync(new SectionCreateViewModel()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SectionCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(await BuildCreateVmAsync(vm));

        var result = await _sectionService.CreateAsync(new Section
        {
            Name = vm.Name,
            SchoolYearId = vm.SchoolYearId,
            GradeLevelId = vm.GradeLevelId,
            CurriculumId = vm.CurriculumId
        }, User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return result.Success ? RedirectToAction(nameof(Details), new { id = result.Data })
                              : View(await BuildCreateVmAsync(vm));
    }

    public async Task<IActionResult> Details(int id)
    {
        var section = await _sectionService.GetWithSchedulesAsync(id);
        if (section is null) return NotFound();

        var vm = new SectionDetailsViewModel
        {
            Section = section,
            AddSchedule = await BuildScheduleVmAsync(new ScheduleCreateViewModel { SectionId = id })
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSchedule([Bind(Prefix = "AddSchedule")] ScheduleCreateViewModel vm)
    {
        if (vm.SubjectId is null)
        {
            TempData["Error"] = "Please select a subject before adding a schedule.";
            return RedirectToAction(nameof(Details), new { id = vm.SectionId });
        }

        var result = await _sectionService.AddScheduleAsync(
            new Schedule { SectionId = vm.SectionId, SubjectId = vm.SubjectId.Value, TeacherId = vm.TeacherId },
            new ScheduleDetail { Day = vm.Day, StartTime = vm.StartTime, EndTime = vm.EndTime, RoomId = vm.RoomId },
            User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = vm.SectionId });
    }

    private async Task<SectionCreateViewModel> BuildCreateVmAsync(SectionCreateViewModel vm)
    {
        vm.SchoolYears = (await _lookup.GetSchoolYearsAsync()).Select(y => new SelectListItem(y.Name, y.SchoolYearId.ToString()));
        vm.GradeLevels = (await _lookup.GetGradeLevelsAsync()).Select(g => new SelectListItem(g.Name, g.GradeLevelId.ToString()));
        vm.Curricula = (await _lookup.GetCurriculaAsync()).Select(c => new SelectListItem(c.Code, c.CurriculumId.ToString()));
        return vm;
    }

    private async Task<ScheduleCreateViewModel> BuildScheduleVmAsync(ScheduleCreateViewModel vm)
    {
        vm.Subjects = (await _lookup.GetSubjectsAsync()).Select(s => new SelectListItem(s.Name, s.SubjectId.ToString()));
        vm.Teachers = (await _lookup.GetTeachersAsync())
            .Select(t => new SelectListItem($"{t.Employee?.FirstName} {t.Employee?.LastName}", t.TeacherId.ToString()));
        vm.Rooms = (await _lookup.GetRoomsAsync()).Select(r => new SelectListItem(r.Name, r.RoomId.ToString()));
        return vm;
    }
}