// File: EnrollmentSystem.UI/Controllers/AdminController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAnnouncementService _announcementService;

    public AdminController(IAuthService authService, IAnnouncementService announcementService)
    {
        _authService = authService;
        _announcementService = announcementService;
    }

    public IActionResult Index() => RedirectToAction(nameof(Users));

    public async Task<IActionResult> Users()
    {
        var users = await _authService.GetAllUsersWithRolesAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        var result = await _authService.ResetPasswordAsync(userId);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            TempData["ResetUserId"] = userId;
            TempData["ResetTempPassword"] = result.Data;
        }
        else { TempData["Error"] = result.Message; }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Calendar(int? year, int? month)
    {
        var anns = await _announcementService.GetAllAsync();
        return View(CalendarBuilder.FromAnnouncements(anns, year, month, "School Calendar"));
    }
}