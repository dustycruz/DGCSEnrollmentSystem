// File: EnrollmentSystem.UI/ViewModels/DashboardViewModel.cs
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.UI.ViewModels;

public class DashboardViewModel
{
    public string Role { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DashboardStatsDto Stats { get; set; } = new();
    public EnrollmentSystem.BLL.DTOs.DashboardChartsDto? Charts { get; set; }
    public IEnumerable<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
}