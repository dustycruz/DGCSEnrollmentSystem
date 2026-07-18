// File: EnrollmentSystem.DAL/Repositories/Interfaces/IEducationalLevelRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IEducationalLevelRepository : IGenericRepository<EducationalLevel>
{
    Task<IEnumerable<EducationalLevel>> GetAllActiveAsync();
    Task<EducationalLevel?> GetByNameAsync(string name);
}