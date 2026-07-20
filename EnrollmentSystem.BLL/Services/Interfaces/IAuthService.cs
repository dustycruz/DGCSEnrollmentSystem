// File: EnrollmentSystem.BLL/Services/Interfaces/IAuthService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<AuthUserDto>> LoginAsync(LoginDto dto);
    Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<ServiceResult<string>> ResetPasswordAsync(string userId);
    Task<IEnumerable<AspNetUser>> GetAllUsersAsync();
    Task<IEnumerable<AuthUserDto>> GetAllUsersWithRolesAsync();
    Task<AuthUserDto?> GetUserAsync(string id);
    Task EnsureSeedDataAsync();
}