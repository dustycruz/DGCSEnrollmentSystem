// File: EnrollmentSystem.UI/Controllers/FacultyController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class FacultyController : Controller
{
    private readonly IAdminService _admin;
    private readonly ITeacherService _teacher;

    public FacultyController(IAdminService admin, ITeacherService teacher)
    {
        _admin = admin;
        _teacher = teacher;
    }

    public async Task<IActionResult> Index()
        => View(await _teacher.GetAllAsync());

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