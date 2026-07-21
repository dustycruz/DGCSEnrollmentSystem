// File: EnrollmentSystem.DAL/Repositories/Implementations/ScheduleRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Schedule>> GetBySectionAsync(int sectionId)
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted && s.SectionId == sectionId)
            .Include(s => s.Subject)
            .Include(s => s.Teacher).ThenInclude(t => t!.Employee)
            .Include(s => s.ScheduleDetails).ThenInclude(d => d.Room)
            .ToListAsync();

    public async Task<IEnumerable<Schedule>> GetByTeacherAsync(int teacherId)
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted && s.TeacherId == teacherId)
            .Include(s => s.Subject)
            .Include(s => s.Section).ThenInclude(sec => sec.GradeLevel)
            .Include(s => s.ScheduleDetails).ThenInclude(d => d.Room)
            .ToListAsync();

    public async Task<Schedule?> GetFullAsync(int scheduleId)
        => await _dbSet
            .Include(s => s.Subject)
            .Include(s => s.Section)
            .Include(s => s.Teacher).ThenInclude(t => t!.Employee)
            .Include(s => s.ScheduleDetails).ThenInclude(d => d.Room)
            .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && !s.IsDeleted);
    public async Task<IEnumerable<Schedule>> GetAllWithDetailsAsync()
    => await _dbSet.AsNoTracking()
        .Where(s => !s.IsDeleted)
        .Include(s => s.Section).ThenInclude(x => x!.GradeLevel)
        .Include(s => s.Subject)
        .Include(s => s.Teacher).ThenInclude(t => t!.Employee)
        .Include(s => s.ScheduleDetails).ThenInclude(d => d.Room)
        .ToListAsync();
}