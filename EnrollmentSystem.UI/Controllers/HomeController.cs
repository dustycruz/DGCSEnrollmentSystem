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
    private readonly IReportingService _reporting;

    public HomeController(IAdminService adminService, IAnnouncementService announcementService, IReportingService reporting)
    {
        _adminService = adminService;
        _announcementService = announcementService;
        _reporting = reporting;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction(nameof(Dashboard));
        return View(await _announcementService.GetRecentAsync(4));
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var role = User.GetPrimaryRole() ?? "User";
        var model = new DashboardViewModel
        {
            Role = role,
            DisplayName = User.GetDisplayName(),
            RecentAnnouncements = await _announcementService.GetRecentAsync(5)
        };

        if (User.IsInRole("Admin"))
        {
            model.Stats = await _adminService.GetDashboardStatsAsync();
            model.Charts = await _reporting.GetDashboardChartsAsync();
        }
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