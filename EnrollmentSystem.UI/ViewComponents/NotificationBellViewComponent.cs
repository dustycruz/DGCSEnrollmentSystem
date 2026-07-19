// File: EnrollmentSystem.UI/ViewComponents/NotificationBellViewComponent.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.ViewComponents;

public class NotificationBellViewComponent : ViewComponent
{
    private readonly INotificationService _notificationService;

    public NotificationBellViewComponent(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return View(new NotificationBellViewModel());

        var items = (await _notificationService.GetForUserAsync(userId, 10)).ToList();
        var unread = await _notificationService.CountUnreadAsync(userId);

        return View(new NotificationBellViewModel { Items = items, UnreadCount = unread });
    }
}