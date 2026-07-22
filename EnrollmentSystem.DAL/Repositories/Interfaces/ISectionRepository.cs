// File: EnrollmentSystem.DAL/Repositories/Interfaces/ISectionRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface ISectionRepository : IGenericRepository<Section>
{
    Task<IEnumerable<Section>> GetAllActiveAsync();
    Task<IEnumerable<Section>> GetBySchoolYearAsync(int schoolYearId);
    Task<IEnumerable<Section>> GetByGradeLevelAsync(int gradeLevelId);
    Task<Section?> GetWithSchedulesAsync(int sectionId);
    Task<IEnumerable<Section>> GetByAdviserAsync(int teacherId);
}