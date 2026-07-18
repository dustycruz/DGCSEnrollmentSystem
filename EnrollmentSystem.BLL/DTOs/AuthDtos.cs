// File: EnrollmentSystem.BLL/DTOs/AuthDtos.cs
namespace EnrollmentSystem.BLL.DTOs;

public class RegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Applicant";
}

public class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
}