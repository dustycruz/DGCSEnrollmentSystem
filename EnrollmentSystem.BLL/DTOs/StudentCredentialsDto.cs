// File: EnrollmentSystem.BLL/DTOs/StudentCredentialsDto.cs
namespace EnrollmentSystem.BLL.DTOs;

public class StudentCredentialsDto
{
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string TempPassword { get; set; } = string.Empty;
}