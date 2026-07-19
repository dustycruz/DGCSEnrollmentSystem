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
    Task<IEnumerable<AspNetUser>> GetAllUsersAsync();
    Task<AuthUserDto?> GetUserAsync(string id);
    Task EnsureSeedDataAsync();
}