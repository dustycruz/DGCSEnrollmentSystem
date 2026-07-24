// File: EnrollmentSystem.UI/Controllers/FacultyController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class FacultyController : Controller
{
    private readonly IAdminService _admin;
    private readonly ITeacherService _teacher;
    private readonly ISectionService _section;

    public FacultyController(IAdminService admin, ITeacherService teacher, ISectionService section)
    {
        _admin = admin;
        _teacher = teacher;
        _section = section;
    }

    public async Task<IActionResult> Index()
    {
        var sections = (await _section.GetAllAsync()).ToList();

        // teacherId -> the class they currently advise (one per teacher)
        ViewBag.AdvisoryByTeacher = sections
            .Where(s => s.AdviserTeacherId.HasValue)
            .GroupBy(s => s.AdviserTeacherId!.Value)
            .ToDictionary(g => g.Key, g => g.First());

        // classes with no adviser yet — the pool a teacher can be assigned to
        ViewBag.FreeSections = sections
            .Where(s => s.AdviserTeacherId is null)
            .Select(s => new SelectListItem(s.Name, s.SectionId.ToString()))
            .ToList();

        return View(await _teacher.GetAllAsync());
    }
    // Teacher profile: account info, advisory, and the classes/subjects they teach.
    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _teacher.GetAsync(id);
        if (teacher?.Employee is null) return NotFound();

        ViewBag.Advisory = (await _teacher.GetAdvisoryAsync(id)).ToList();
        ViewBag.Schedules = (await _teacher.GetScheduleAsync(id)).ToList();
        return View(teacher);
    }

    // Assign a class advisory to a teacher (guarded to one advisory per teacher in the service).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignAdvisory(int teacherId, int sectionId)
    {
        var result = await _section.AssignAdviserAsync(sectionId, teacherId, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // Remove a teacher's advisory by clearing the adviser on that class.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAdvisory(int sectionId)
    {
        var result = await _section.AssignAdviserAsync(sectionId, null, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create() => View(new TeacherCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _admin.CreateTeacherAsync(new Employee
        {
            FirstName = vm.FirstName,
            MiddleName = vm.MiddleName,
            LastName = vm.LastName,
            EmployeeNumber = vm.EmployeeNumber,
            EmailAddress = vm.EmailAddress
        }, User.GetUserName());

        if (result.Success && result.Data is not null)
        {
            TempData["Success"] = result.Message;
            TempData["NewTeacherUser"] = result.Data.UserName;
            TempData["NewTeacherPass"] = result.Data.TempPassword;
        }
        else { TempData["Error"] = result.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _teacher.GetAsync(id);
        if (teacher?.Employee is null) return NotFound();

        return View(new TeacherEditViewModel
        {
            TeacherId = teacher.TeacherId,
            FirstName = teacher.Employee.FirstName,
            MiddleName = teacher.Employee.MiddleName,
            LastName = teacher.Employee.LastName,
            EmployeeNumber = teacher.Employee.EmployeeNumber,
            EmailAddress = teacher.Employee.EmailAddress
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TeacherEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _admin.UpdateTeacherAsync(vm.TeacherId, new Employee
        {
            FirstName = vm.FirstName,
            MiddleName = vm.MiddleName,
            LastName = vm.LastName,
            EmailAddress = vm.EmailAddress
        }, User.GetUserName());

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _admin.DeleteTeacherAsync(id, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}