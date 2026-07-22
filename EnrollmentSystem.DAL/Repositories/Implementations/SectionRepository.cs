// File: EnrollmentSystem.DAL/Repositories/Implementations/SectionRepository.cs
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class SectionRepository : GenericRepository<Section>, ISectionRepository
{
    public SectionRepository(EnrollmentSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Section>> GetAllActiveAsync()
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Include(s => s.GradeLevel)
            .Include(s => s.SchoolYear)
            .Include(s => s.Curriculum)
            .Include(s => s.AdviserTeacher).ThenInclude(t => t!.Employee)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public async Task<IEnumerable<Section>> GetBySchoolYearAsync(int schoolYearId)
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted && s.SchoolYearId == schoolYearId)
            .Include(s => s.GradeLevel)
            .ToListAsync();

    public async Task<IEnumerable<Section>> GetByGradeLevelAsync(int gradeLevelId)
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted && s.GradeLevelId == gradeLevelId)
            .Include(s => s.SchoolYear)
            .ToListAsync();

    public async Task<Section?> GetWithSchedulesAsync(int sectionId)
        => await _dbSet
            .Include(s => s.GradeLevel)
            .Include(s => s.SchoolYear)
            .Include(s => s.AdviserTeacher).ThenInclude(t => t!.Employee)
            .Include(s => s.Schedules).ThenInclude(sc => sc.Subject)
            .Include(s => s.Schedules).ThenInclude(sc => sc.Teacher).ThenInclude(t => t!.Employee)
            .Include(s => s.Schedules).ThenInclude(sc => sc.ScheduleDetails).ThenInclude(sd => sd.Room)
            .FirstOrDefaultAsync(s => s.SectionId == sectionId && !s.IsDeleted);

    public async Task<IEnumerable<Section>> GetByAdviserAsync(int teacherId)
        => await _dbSet.AsNoTracking()
            .Where(s => !s.IsDeleted && s.AdviserTeacherId == teacherId)
            .Include(s => s.GradeLevel)
            .Include(s => s.SchoolYear)
            .ToListAsync();
}