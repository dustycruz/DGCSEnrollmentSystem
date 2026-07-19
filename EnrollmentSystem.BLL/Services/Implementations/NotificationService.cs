// File: EnrollmentSystem.BLL/Services/Implementations/NotificationService.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _repo;

    public NotificationService(IGenericRepository<Notification> repo)
    {
        _repo = repo;
    }

    public async Task NotifyAsync(string userId, string title, string? message)
    {
        await _repo.AddAsync(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.Now
        });
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<Notification>> GetForUserAsync(string userId, int count = 20)
        => (await _repo.FindAsync(n => n.UserId == userId))
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToList();

    public async Task<int> CountUnreadAsync(string userId)
        => await _repo.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkAllReadAsync(string userId)
    {
        var unread = await _repo.FindAsync(n => n.UserId == userId && !n.IsRead);
        foreach (var n in unread)
        {
            n.IsRead = true;
            _repo.Update(n);
        }
        await _repo.SaveAsync();
    }
}