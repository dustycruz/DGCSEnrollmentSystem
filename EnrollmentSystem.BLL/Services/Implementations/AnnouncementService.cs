// File: EnrollmentSystem.BLL/Services/Implementations/AnnouncementService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly IAuditService _audit;

    public AnnouncementService(IAnnouncementRepository announcementRepo, IAuditService audit)
    {
        _announcementRepo = announcementRepo;
        _audit = audit;
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

        await _audit.LogAsync(
            action: "Announcement Posted",
            entityName: "Announcement",
            entityId: announcement.AnnouncementId.ToString(),
            description: $"Announcement \"{announcement.Title}\" posted by {createdBy}.",
            status: "Posted");

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

        await _audit.LogAsync(
            action: "Announcement Deleted",
            entityName: "Announcement",
            entityId: existing.AnnouncementId.ToString(),
            description: $"Announcement \"{existing.Title}\" deleted by {modifiedBy}.",
            status: "Deleted");

        return ServiceResult.Ok("Announcement deleted.");
    }
    public async Task<IEnumerable<Announcement>> GetFeedForSectionAsync(int? sectionId)
    {
        var recent = await _announcementRepo.GetRecentAsync(200);
        return recent
            .Where(a => a.SectionId == null || (sectionId.HasValue && a.SectionId == sectionId.Value))
            .OrderByDescending(a => a.PostedDate)
            .ToList();
    }
}