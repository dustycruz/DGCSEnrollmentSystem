// File: EnrollmentSystem.UI/Controllers/HomeController.cs
using System.Diagnostics;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

public class HomeController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IAnnouncementService _announcementService;

    public HomeController(IAdminService adminService, IAnnouncementService announcementService)
    {
        _adminService = adminService;
        _announcementService = announcementService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction(nameof(Dashboard));

        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var role = User.GetPrimaryRole() ?? "User";

        var model = new DashboardViewModel
        {
            Role = role,
            DisplayName = User.GetUserName(),
            RecentAnnouncements = await _announcementService.GetRecentAsync(5)
        };

        if (User.IsInRole("Admin"))
            model.Stats = await _adminService.GetDashboardStatsAsync();

        return View(model);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View();
    }
}