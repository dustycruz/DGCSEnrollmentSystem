// File: EnrollmentSystem.BLL/Services/Implementations/ApplicantOnboardingService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class ApplicantOnboardingService : IApplicantOnboardingService
{
    private readonly IGenericRepository<EmailVerification> _verificationRepo;
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;

    public ApplicantOnboardingService(
        IGenericRepository<EmailVerification> verificationRepo,
        IUserRepository userRepo,
        IEmailService emailService)
    {
        _verificationRepo = verificationRepo;
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public async Task<ServiceResult> RequestVerificationAsync(string email)
    {
        email = (email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return ServiceResult.Fail("Please enter a valid email address.");

        if (await _userRepo.EmailExistsAsync(email))
            return ServiceResult.Fail("This email is already registered to an applicant or student account.");

        var code = CredentialGenerator.GenerateOtp();
        await _verificationRepo.AddAsync(new EmailVerification
        {
            Email = email,
            Code = code,
            ExpiresAt = DateTime.Now.AddMinutes(10),
            IsVerified = false,
            CreatedAt = DateTime.Now
        });
        await _verificationRepo.SaveAsync();

        await _emailService.SendOtpAsync(email, code);
        return ServiceResult.Ok("A verification code has been sent to your email.");
    }

    public async Task<ServiceResult> VerifyEmailAsync(string email, string code)
    {
        email = (email ?? string.Empty).Trim().ToLowerInvariant();
        var record = (await _verificationRepo.FindAsync(v => v.Email == email && v.Code == code))
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();

        if (record is null)
            return ServiceResult.Fail("Invalid verification code.");
        if (record.ExpiresAt < DateTime.Now)
            return ServiceResult.Fail("This code has expired. Please request a new one.");

        record.IsVerified = true;
        _verificationRepo.Update(record);
        await _verificationRepo.SaveAsync();

        return ServiceResult.Ok("Email verified.");
    }

    public async Task<ServiceResult<string>> CompleteRegistrationAsync(string email, string password)
    {
        email = (email ?? string.Empty).Trim().ToLowerInvariant();

        var verified = (await _verificationRepo.FindAsync(v => v.Email == email && v.IsVerified))
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();
        if (verified is null)
            return ServiceResult<string>.Fail("Email has not been verified.");

        if (await _userRepo.EmailExistsAsync(email))
            return ServiceResult<string>.Fail("This email is already registered.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return ServiceResult<string>.Fail("Password must be at least 6 characters.");

        // Applicant Number: APP-YYYY-000001
        var prefix = $"APP-{DateTime.Now.Year}-";
        var last = await _userRepo.GetLastUserNameByPrefixAsync(prefix);
        var next = 1;
        if (!string.IsNullOrEmpty(last) && int.TryParse(last.Substring(prefix.Length), out var parsed))
            next = parsed + 1;
        var applicantNumber = $"{prefix}{next:D6}";

        var hash = PasswordHasher.Hash(password);
        var user = new AspNetUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = applicantNumber,
            Email = email,
            PasswordHash = hash,
            EmailConfirmed = true,
            MustChangePassword = false   // applicant chose their own password
        };

        var role = await _userRepo.EnsureRoleAsync("Applicant");
        await _userRepo.AddUserAsync(user);
        await _userRepo.AddUserToRoleAsync(user.Id, role.Id, applicantNumber, hash);
        await _userRepo.SaveAsync();

        await _emailService.SendApplicantNumberAsync(email, applicantNumber);
        return ServiceResult<string>.Ok(applicantNumber, "Account created. Your Applicant Number has been emailed to you.");
    }
    public async Task<string?> PeekLatestCodeAsync(string email)
    {
        email = (email ?? string.Empty).Trim().ToLowerInvariant();
        var record = (await _verificationRepo.FindAsync(
                v => v.Email == email && !v.IsVerified && v.ExpiresAt > DateTime.Now))
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();
        return record?.Code;
    }
}