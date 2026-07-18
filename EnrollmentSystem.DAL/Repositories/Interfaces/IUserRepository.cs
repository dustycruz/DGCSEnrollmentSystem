// File: EnrollmentSystem.DAL/Repositories/Interfaces/IUserRepository.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AspNetUser?> GetByUsernameAsync(string userName);
    Task<AspNetUser?> GetByEmailAsync(string email);
    Task<AspNetUser?> GetByIdAsync(string id);
    Task<IEnumerable<AspNetUser>> GetAllAsync();
    Task<IEnumerable<string>> GetUserRoleNamesAsync(string userId);
    Task<AspNetRole?> GetRoleByNameAsync(string roleName);
    Task<AspNetRole> EnsureRoleAsync(string roleName);
    Task AddUserAsync(AspNetUser user);
    Task AddUserToRoleAsync(string userId, string roleId, string userName, string passwordHash);
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> EmailExistsAsync(string email);
    Task<int> SaveAsync();
}