// File: EnrollmentSystem.BLL/Services/Implementations/EmailService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        // Dev fallback: if SMTP isn't configured, log the email instead of sending.
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.UserName))
        {
            _logger.LogWarning("SMTP disabled. Email to {To} | Subject: {Subject} | Body: {Body}",
                toEmail, subject, htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName,
            string.IsNullOrWhiteSpace(_settings.FromAddress) ? _settings.UserName : _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.UserName, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public Task SendOtpAsync(string toEmail, string code)
        => SendAsync(toEmail, "DGCS Email Verification Code",
            $"<h2>Divine Grace Center of Studies</h2>" +
            $"<p>Your verification code is:</p>" +
            $"<h1 style='letter-spacing:6px'>{code}</h1>" +
            $"<p>This code expires in 10 minutes.</p>");

    public Task SendApplicantNumberAsync(string toEmail, string applicantNumber)
        => SendAsync(toEmail, "Your DGCS Applicant Number",
            $"<h2>Welcome to Divine Grace Center of Studies!</h2>" +
            $"<p>Your Applicant Number is:</p><h1>{applicantNumber}</h1>" +
            $"<p>Use this number together with the password you created to sign in to the Applicant Portal " +
            $"and complete your admission application.</p>");

    public Task SendStatusUpdateAsync(string toEmail, string status, string? remarks)
        => SendAsync(toEmail, $"DGCS Application Update: {status}",
            $"<h2>Application Status Update</h2>" +
            $"<p>Your application status is now: <strong>{status}</strong></p>" +
            (string.IsNullOrWhiteSpace(remarks) ? "" : $"<p>Remarks from the registrar: {remarks}</p>") +
            $"<p>Sign in to the Applicant Portal for details.</p>");

    public Task SendStudentCredentialsAsync(string toEmail, string studentNumber, string tempPassword)
        => SendAsync(toEmail, "Welcome to DGCS — Your Student Account",
            $"<h2>Congratulations, you are officially enrolled!</h2>" +
            $"<p>Your Student Number: <strong>{studentNumber}</strong><br/>" +
            $"Temporary Password: <strong>{tempPassword}</strong></p>" +
            $"<p>Sign in to the Student Portal with these credentials. " +
            $"You will be required to change your password on first login.</p>");
}