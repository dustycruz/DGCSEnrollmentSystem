// File: EnrollmentSystem.BLL/Services/Interfaces/INotificationService.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(string userId, string title, string? message);
    Task<IEnumerable<Notification>> GetForUserAsync(string userId, int count = 20);
    Task<int> CountUnreadAsync(string userId);
    Task MarkAllReadAsync(string userId);
}