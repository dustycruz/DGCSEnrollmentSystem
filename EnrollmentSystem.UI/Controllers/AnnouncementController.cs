// File: EnrollmentSystem.UI/Controllers/AnnouncementController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class AnnouncementController : Controller
{
    private readonly IAnnouncementService _announcementService;

    public AnnouncementController(IAnnouncementService announcementService)
    {
        _announcementService = announcementService;
    }

    public async Task<IActionResult> Index()
        => View(await _announcementService.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new AnnouncementCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnnouncementCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _announcementService.CreateAsync(
            new Announcement { Title = vm.Title, Content = vm.Content }, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _announcementService.DeleteAsync(id, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}