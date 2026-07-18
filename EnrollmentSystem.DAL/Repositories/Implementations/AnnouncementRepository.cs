// File: EnrollmentSystem.DAL/Repositories/Implementations/AnnouncementRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class AnnouncementRepository : GenericRepository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Announcement>> GetRecentAsync(int count)
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.PostedDate)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Announcement>> GetBySectionAsync(int sectionId)
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted && a.SectionId == sectionId)
            .OrderByDescending(a => a.PostedDate)
            .ToListAsync();

    public async Task<IEnumerable<Announcement>> GetByTeacherAsync(int teacherId)
        => await _dbSet.AsNoTracking()
            .Where(a => !a.IsDeleted && a.TeacherId == teacherId)
            .OrderByDescending(a => a.PostedDate)
            .ToListAsync();
}