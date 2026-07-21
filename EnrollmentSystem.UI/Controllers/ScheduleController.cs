// File: EnrollmentSystem.UI/Controllers/ScheduleController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class ScheduleController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly ILookupService _lookup;

    public ScheduleController(ISectionService sectionService, ILookupService lookup)
    {
        _sectionService = sectionService;
        _lookup = lookup;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new ScheduleManageViewModel
        {
            Schedules = await _sectionService.GetAllSchedulesAsync(),
            Sections = (await _lookup.GetSectionsAsync()).Select(s => new SelectListItem(s.Name, s.SectionId.ToString())),
            AddForm = new ScheduleCreateViewModel
            {
                Subjects = (await _lookup.GetSubjectsAsync()).Select(s => new SelectListItem(s.Name, s.SubjectId.ToString())),
                Teachers = (await _lookup.GetTeachersAsync()).Select(t => new SelectListItem($"{t.Employee?.FirstName} {t.Employee?.LastName}", t.TeacherId.ToString())),
                Rooms = (await _lookup.GetRoomsAsync()).Select(r => new SelectListItem(r.Name, r.RoomId.ToString()))
            }
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind(Prefix = "AddForm")] ScheduleCreateViewModel vm)
    {
        if (vm.SectionId <= 0) { TempData["Error"] = "Please select a section."; return RedirectToAction(nameof(Index)); }
        if (vm.SubjectId is null) { TempData["Error"] = "Please select a subject."; return RedirectToAction(nameof(Index)); }
        if (string.IsNullOrWhiteSpace(vm.Day)) { TempData["Error"] = "Please select a day."; return RedirectToAction(nameof(Index)); }
        if (vm.StartTime is null || vm.EndTime is null) { TempData["Error"] = "Please set start and end times."; return RedirectToAction(nameof(Index)); }
        if (vm.EndTime <= vm.StartTime) { TempData["Error"] = "End time must be after the start time."; return RedirectToAction(nameof(Index)); }

        var result = await _sectionService.AddScheduleAsync(
            new Schedule { SectionId = vm.SectionId, SubjectId = vm.SubjectId.Value, TeacherId = vm.TeacherId },
            new ScheduleDetail { Day = vm.Day, StartTime = vm.StartTime, EndTime = vm.EndTime, RoomId = vm.RoomId },
            User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}