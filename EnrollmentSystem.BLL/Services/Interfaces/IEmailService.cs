// File: EnrollmentSystem.BLL/Services/Interfaces/IEmailService.cs
namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
    Task SendOtpAsync(string toEmail, string code);
    Task SendApplicantNumberAsync(string toEmail, string applicantNumber);
    Task SendStatusUpdateAsync(string toEmail, string status, string? remarks);
    Task SendStudentCredentialsAsync(string toEmail, string studentNumber, string tempPassword);
}