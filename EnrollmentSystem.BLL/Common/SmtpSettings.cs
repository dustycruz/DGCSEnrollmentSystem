// File: EnrollmentSystem.BLL/Common/SmtpSettings.cs
namespace EnrollmentSystem.BLL.Common;

public class SmtpSettings
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string UserName { get; set; } = string.Empty;   // your Gmail address
    public string Password { get; set; } = string.Empty;   // Gmail App Password
    public string FromName { get; set; } = "DGCS Enrollment System";
    public string FromAddress { get; set; } = string.Empty;
}