// File: EnrollmentSystem.UI/ViewModels/CalendarViewModel.cs
namespace EnrollmentSystem.UI.ViewModels;

public class CalendarEventItem
{
    public string? WeeklyDay { get; set; }
    public DateOnly? OnDate { get; set; }
    public TimeOnly? Time { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class CalendarViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Title { get; set; } = "Calendar";
    public List<CalendarEventItem> Events { get; set; } = new();
}