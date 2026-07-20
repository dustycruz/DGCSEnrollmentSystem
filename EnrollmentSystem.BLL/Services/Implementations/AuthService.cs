// File: EnrollmentSystem.BLL/Services/Implementations/AuthService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;

    public AuthService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<ServiceResult> RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
            return ServiceResult.Fail("Username and password are required.");

        if (await _userRepo.UserNameExistsAsync(dto.UserName))
            return ServiceResult.Fail("Username is already taken.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _userRepo.EmailExistsAsync(dto.Email))
            return ServiceResult.Fail("Email is already registered.");

        var roleName = string.IsNullOrWhiteSpace(dto.Role) ? "Applicant" : dto.Role;
        var role = await _userRepo.EnsureRoleAsync(roleName);

        var hash = PasswordHasher.Hash(dto.Password);
        var user = new AspNetUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = hash
        };

        await _userRepo.AddUserAsync(user);
        await _userRepo.AddUserToRoleAsync(user.Id, role.Id, user.UserName!, hash);
        await _userRepo.SaveAsync();

        return ServiceResult.Ok("Registration successful.");
    }

    public async Task<ServiceResult<AuthUserDto>> LoginAsync(LoginDto dto)
    {
        // Allow sign-in with either username (Applicant/Student Number) OR email
        var user = await _userRepo.GetByUsernameAsync(dto.UserName)
                   ?? await _userRepo.GetByEmailAsync(dto.UserName);

        if (user is null || string.IsNullOrEmpty(user.PasswordHash) ||
            !PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return ServiceResult<AuthUserDto>.Fail("Invalid username/email or password.");
        }

        var roles = (await _userRepo.GetUserRoleNamesAsync(user.Id)).ToList();
        var result = new AuthUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            Roles = roles,
            MustChangePassword = user.MustChangePassword
        };

        return ServiceResult<AuthUserDto>.Ok(result, "Login successful.");
    }

    public async Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
            return ServiceResult.Fail("User not found.");

        if (!PasswordHasher.Verify(currentPassword, user.PasswordHash))
            return ServiceResult.Fail("Current password is incorrect.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return ServiceResult.Fail("New password must be at least 6 characters.");

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        user.MustChangePassword = false;
        await _userRepo.SaveAsync();

        return ServiceResult.Ok("Password changed successfully.");
    }

    public async Task<IEnumerable<AspNetUser>> GetAllUsersAsync()
        => await _userRepo.GetAllAsync();

    public async Task<AuthUserDto?> GetUserAsync(string id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null) return null;

        var roles = (await _userRepo.GetUserRoleNamesAsync(user.Id)).ToList();
        return new AuthUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            Roles = roles,
            MustChangePassword = user.MustChangePassword
        };
    }

    public async Task EnsureSeedDataAsync()
    {
        foreach (var role in new[] { "Admin", "Teacher", "Student", "Applicant" })
            await _userRepo.EnsureRoleAsync(role);

        if (!await _userRepo.UserNameExistsAsync("admin"))
        {
            await RegisterAsync(new RegisterDto
            {
                UserName = "admin",
                Email = "admin@dgcs.edu.ph",
                Password = "Admin@123",
                Role = "Admin"
            });
        }
    }
    public async Task<ServiceResult<string>> ResetPasswordAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return ServiceResult<string>.Fail("User not found.");

        var temp = CredentialGenerator.GenerateTempPassword();
        user.PasswordHash = PasswordHasher.Hash(temp);
        user.MustChangePassword = true;
        await _userRepo.SaveAsync();

        return ServiceResult<string>.Ok(temp, "Password reset. A new temporary password was generated.");
    }

    public async Task<IEnumerable<AuthUserDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userRepo.GetAllAsync();
        var list = new List<AuthUserDto>();
        foreach (var u in users)
        {
            var roles = (await _userRepo.GetUserRoleNamesAsync(u.Id)).ToList();
            list.Add(new AuthUserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email,
                Roles = roles,
                MustChangePassword = u.MustChangePassword
            });
        }
        return list;
    }
}