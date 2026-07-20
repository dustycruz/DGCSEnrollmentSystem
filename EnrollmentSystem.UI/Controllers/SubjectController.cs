// File: EnrollmentSystem.UI/Controllers/SubjectController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class SubjectController : Controller
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
        => View(await _subjectService.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new SubjectCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _subjectService.CreateAsync(new Subject { Name = vm.Name, Code = vm.Code }, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subjectService.DeleteAsync(id, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}