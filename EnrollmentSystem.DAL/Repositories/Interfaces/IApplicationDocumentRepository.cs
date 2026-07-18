// File: EnrollmentSystem.DAL/Repositories/Interfaces/IApplicationDocumentRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IApplicationDocumentRepository : IGenericRepository<ApplicationDocument>
{
    Task<IEnumerable<ApplicationDocument>> GetByApplicationAsync(int applicationId);
}