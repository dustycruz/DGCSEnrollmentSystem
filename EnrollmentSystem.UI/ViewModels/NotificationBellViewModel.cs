// File: EnrollmentSystem.UI/ViewModels/NotificationBellViewModel.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class NotificationBellViewModel
{
    public int UnreadCount { get; set; }
    public List<Notification> Items { get; set; } = new();
}