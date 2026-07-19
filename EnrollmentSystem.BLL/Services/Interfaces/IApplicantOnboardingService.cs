// File: EnrollmentSystem.BLL/Services/Interfaces/IApplicantOnboardingService.cs
using EnrollmentSystem.BLL.Common;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IApplicantOnboardingService
{
    /// <summary>Step 2-3: validate email is unused, generate OTP, email it.</summary>
    Task<ServiceResult> RequestVerificationAsync(string email);

    /// <summary>Step 3: check the OTP the applicant typed.</summary>
    Task<ServiceResult> VerifyEmailAsync(string email, string code);

    /// <summary>Step 4: applicant sets own password; account + Applicant Number created and emailed.</summary>
    Task<ServiceResult<string>> CompleteRegistrationAsync(string email, string password);
    /// <summary>Demo/dev only: returns the latest active code so the UI can display it when email is off.</summary>
    Task<string?> PeekLatestCodeAsync(string email);
}