// File: EnrollmentSystem.BLL/DTOs/AccountCredentialsDto.cs
namespace EnrollmentSystem.BLL.DTOs;

public class AccountCredentialsDto
{
    public string UserName { get; set; } = string.Empty;
    public string TempPassword { get; set; } = string.Empty;
}