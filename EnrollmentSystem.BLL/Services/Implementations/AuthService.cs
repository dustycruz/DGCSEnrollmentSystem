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
        // Allow sign-in with either username OR email
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
            Roles = roles
        };

        return ServiceResult<AuthUserDto>.Ok(result, "Login successful.");
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
            Roles = roles
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
}