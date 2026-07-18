// File: EnrollmentSystem.DAL/Repositories/Interfaces/ICurriculumRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface ICurriculumRepository : IGenericRepository<Curriculum>
{
    Task<IEnumerable<Curriculum>> GetAllActiveAsync();
    Task<Curriculum?> GetWithSubjectsAsync(int curriculumId);
    Task<IEnumerable<Curriculum>> GetByEducationalLevelAsync(int educationalLevelId);
}