// File: EnrollmentSystem.UI/Helpers/CalendarBuilder.cs
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.ViewModels;

namespace EnrollmentSystem.UI.Helpers;

public static class CalendarBuilder
{
    public static CalendarViewModel FromSchedules(IEnumerable<Schedule> schedules, int? year, int? month, string title)
    {
        var vm = new CalendarViewModel { Year = year ?? DateTime.Today.Year, Month = month ?? DateTime.Today.Month, Title = title };
        foreach (var s in schedules)
            foreach (var d in s.ScheduleDetails)
                vm.Events.Add(new CalendarEventItem
                {
                    WeeklyDay = d.Day,
                    Time = d.StartTime,
                    Label = s.Subject?.Code ?? s.Subject?.Name ?? "Class"
                });
        return vm;
    }

    public static CalendarViewModel FromAnnouncements(IEnumerable<Announcement> anns, int? year, int? month, string title)
    {
        var vm = new CalendarViewModel { Year = year ?? DateTime.Today.Year, Month = month ?? DateTime.Today.Month, Title = title };
        foreach (var a in anns)
            vm.Events.Add(new CalendarEventItem { OnDate = DateOnly.FromDateTime(a.PostedDate), Label = a.Title });
        return vm;
    }
}