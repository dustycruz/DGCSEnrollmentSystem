// File: EnrollmentSystem.DAL/Repositories/Interfaces/IEmployeeRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<IEnumerable<Employee>> GetAllActiveAsync();
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<Employee>> SearchAsync(string term);
}