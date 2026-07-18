// File: EnrollmentSystem.DAL/Repositories/Interfaces/ISubjectRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<IEnumerable<Subject>> GetAllActiveAsync();
    Task<Subject?> GetByCodeAsync(string code);
}