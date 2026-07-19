// File: EnrollmentSystem.UI/Controllers/NotificationController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var items = await _notificationService.GetForUserAsync(userId, 100);
        await _notificationService.MarkAllReadAsync(userId);
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = User.GetUserId();
        if (!string.IsNullOrEmpty(userId))
            await _notificationService.MarkAllReadAsync(userId);

        var referer = Request.Headers["Referer"].ToString();
        return string.IsNullOrEmpty(referer) ? RedirectToAction("Dashboard", "Home") : Redirect(referer);
    }
}