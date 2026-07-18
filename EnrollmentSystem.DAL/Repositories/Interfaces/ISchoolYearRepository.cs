// File: EnrollmentSystem.DAL/Repositories/Interfaces/ISchoolYearRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface ISchoolYearRepository : IGenericRepository<SchoolYear>
{
    Task<IEnumerable<SchoolYear>> GetAllActiveAsync();
    Task<SchoolYear?> GetByNameAsync(string name);
    Task<SchoolYear?> GetLatestAsync();
}