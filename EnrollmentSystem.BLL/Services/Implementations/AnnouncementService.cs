// File: EnrollmentSystem.BLL/Services/Implementations/AnnouncementService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _announcementRepo;

    public AnnouncementService(IAnnouncementRepository announcementRepo)
    {
        _announcementRepo = announcementRepo;
    }

    public async Task<IEnumerable<Announcement>> GetAllAsync()
        => await _announcementRepo.GetRecentAsync(int.MaxValue);

    public async Task<IEnumerable<Announcement>> GetRecentAsync(int count)
        => await _announcementRepo.GetRecentAsync(count);

    public async Task<IEnumerable<Announcement>> GetBySectionAsync(int sectionId)
        => await _announcementRepo.GetBySectionAsync(sectionId);

    public async Task<IEnumerable<Announcement>> GetByTeacherAsync(int teacherId)
        => await _announcementRepo.GetByTeacherAsync(teacherId);

    public async Task<ServiceResult<int>> CreateAsync(Announcement announcement, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(announcement.Title))
            return ServiceResult<int>.Fail("Announcement title is required.");

        announcement.PostedDate = DateTime.Now;
        announcement.CreatedBy = createdBy;
        announcement.IsDeleted = false;

        await _announcementRepo.AddAsync(announcement);
        await _announcementRepo.SaveAsync();

        return ServiceResult<int>.Ok(announcement.AnnouncementId, "Announcement posted.");
    }

    public async Task<ServiceResult> DeleteAsync(int id, string modifiedBy)
    {
        var existing = await _announcementRepo.GetByIdAsync(id);
        if (existing is null)
            return ServiceResult.Fail("Announcement not found.");

        existing.IsDeleted = true;
        existing.ModifiedBy = modifiedBy;
        _announcementRepo.Update(existing);
        await _announcementRepo.SaveAsync();

        return ServiceResult.Ok("Announcement deleted.");
    }
}