// File: EnrollmentSystem.DAL/Repositories/Interfaces/IGradeLevelRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IGradeLevelRepository : IGenericRepository<GradeLevel>
{
    Task<IEnumerable<GradeLevel>> GetAllActiveAsync();
    Task<IEnumerable<GradeLevel>> GetByEducationalLevelAsync(int educationalLevelId);
}