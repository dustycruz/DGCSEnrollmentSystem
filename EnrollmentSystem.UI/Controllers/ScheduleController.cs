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
            AddForm = await BuildFormAsync(new ScheduleCreateViewModel())
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind(Prefix = "AddForm")] ScheduleCreateViewModel vm)
    {
        var error = Validate(vm, requireSection: true);
        if (error is not null) { TempData["Error"] = error; return RedirectToAction(nameof(Index)); }

        var result = await _sectionService.AddScheduleAsync(
            new Schedule { SectionId = vm.SectionId, SubjectId = vm.SubjectId!.Value, TeacherId = vm.TeacherId },
            new ScheduleDetail { Day = vm.Day, StartTime = vm.StartTime, EndTime = vm.EndTime, RoomId = vm.RoomId },
            User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await _sectionService.GetScheduleAsync(id);
        if (s is null) return NotFound();
        var d = s.ScheduleDetails.FirstOrDefault();

        var vm = await BuildFormAsync(new ScheduleCreateViewModel
        {
            ScheduleId = s.ScheduleId,
            SectionId = s.SectionId,
            SectionName = s.Section?.Name,
            SubjectId = s.SubjectId,
            TeacherId = s.TeacherId,
            Day = d?.Day,
            StartTime = d?.StartTime,
            EndTime = d?.EndTime,
            RoomId = d?.RoomId
        });
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ScheduleCreateViewModel vm)
    {
        var error = Validate(vm, requireSection: false);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(await BuildFormAsync(vm));
        }

        var result = await _sectionService.UpdateScheduleAsync(
            vm.ScheduleId, vm.SubjectId!.Value, vm.TeacherId,
            new ScheduleDetail { Day = vm.Day, StartTime = vm.StartTime, EndTime = vm.EndTime, RoomId = vm.RoomId },
            User.GetUserName());

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Update failed.");
            return View(await BuildFormAsync(vm));
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sectionService.DeleteScheduleAsync(id, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private static string? Validate(ScheduleCreateViewModel vm, bool requireSection)
    {
        if (requireSection && vm.SectionId <= 0) return "Please select a section.";
        if (vm.SubjectId is null) return "Please select a subject.";
        if (string.IsNullOrWhiteSpace(vm.Day)) return "Please select a day.";
        if (vm.StartTime is null || vm.EndTime is null) return "Please set start and end times.";
        if (vm.EndTime <= vm.StartTime) return "End time must be after the start time.";
        return null;
    }

    private async Task<ScheduleCreateViewModel> BuildFormAsync(ScheduleCreateViewModel vm)
    {
        vm.Subjects = (await _lookup.GetSubjectsAsync()).Select(s => new SelectListItem(s.Name, s.SubjectId.ToString()));
        vm.Teachers = (await _lookup.GetTeachersAsync()).Select(t => new SelectListItem($"{t.Employee?.FirstName} {t.Employee?.LastName}", t.TeacherId.ToString()));
        vm.Rooms = (await _lookup.GetRoomsAsync()).Select(r => new SelectListItem(r.Name, r.RoomId.ToString()));
        return vm;
    }
}