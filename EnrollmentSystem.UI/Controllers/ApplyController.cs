// File: EnrollmentSystem.UI/Controllers/ApplyController.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnrollmentSystem.UI.Controllers;

[AllowAnonymous]
public class ApplyController : Controller
{
    private readonly IApplicantOnboardingService _onboarding;
    private readonly SmtpSettings _smtp;

    private const string SessionEmail = "onb_email";
    private const string SessionVerified = "onb_verified";

    public ApplyController(IApplicantOnboardingService onboarding, IOptions<SmtpSettings> smtp)
    {
        _onboarding = onboarding;
        _smtp = smtp.Value;
    }

    // STEP 1 — email entry
    [HttpGet]
    public IActionResult Index() => View(new EmailEntryViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(EmailEntryViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _onboarding.RequestVerificationAsync(model.Email);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Could not send code.");
            return View(model);
        }

        HttpContext.Session.SetString(SessionEmail, model.Email.Trim().ToLowerInvariant());
        HttpContext.Session.Remove(SessionVerified);
        TempData["Info"] = result.Message;
        return RedirectToAction(nameof(Verify));
    }

    // STEP 2 — OTP
    [HttpGet]
    public async Task<IActionResult> Verify()
    {
        var email = HttpContext.Session.GetString(SessionEmail);
        if (string.IsNullOrEmpty(email)) return RedirectToAction(nameof(Index));

        // Demo mode: when email sending is off, show the code on-screen.
        if (!_smtp.Enabled)
            ViewData["DemoCode"] = await _onboarding.PeekLatestCodeAsync(email);

        return View(new OtpViewModel { Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(OtpViewModel model)
    {
        var email = HttpContext.Session.GetString(SessionEmail);
        if (string.IsNullOrEmpty(email)) return RedirectToAction(nameof(Index));
        model.Email = email;

        if (!ModelState.IsValid)
        {
            if (!_smtp.Enabled)
                ViewData["DemoCode"] = await _onboarding.PeekLatestCodeAsync(email);
            return View(model);
        }

        var result = await _onboarding.VerifyEmailAsync(email, model.Code.Trim());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Invalid code.");
            if (!_smtp.Enabled)
                ViewData["DemoCode"] = await _onboarding.PeekLatestCodeAsync(email);
            return View(model);
        }

        HttpContext.Session.SetString(SessionVerified, "true");
        return RedirectToAction(nameof(SetPassword));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resend()
    {
        var email = HttpContext.Session.GetString(SessionEmail);
        if (string.IsNullOrEmpty(email)) return RedirectToAction(nameof(Index));

        var result = await _onboarding.RequestVerificationAsync(email);
        TempData["Info"] = result.Success ? "A new code has been sent." : result.Message;
        return RedirectToAction(nameof(Verify));
    }

    // STEP 3 — set password
    [HttpGet]
    public IActionResult SetPassword()
    {
        var email = HttpContext.Session.GetString(SessionEmail);
        var verified = HttpContext.Session.GetString(SessionVerified);
        if (string.IsNullOrEmpty(email) || verified != "true")
            return RedirectToAction(nameof(Index));

        return View(new SetPasswordViewModel { Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        var email = HttpContext.Session.GetString(SessionEmail);
        var verified = HttpContext.Session.GetString(SessionVerified);
        if (string.IsNullOrEmpty(email) || verified != "true")
            return RedirectToAction(nameof(Index));

        model.Email = email;
        if (!ModelState.IsValid) return View(model);

        var result = await _onboarding.CompleteRegistrationAsync(email, model.Password);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed.");
            return View(model);
        }

        HttpContext.Session.Remove(SessionEmail);
        HttpContext.Session.Remove(SessionVerified);
        TempData["ApplicantNumber"] = result.Data;
        return RedirectToAction(nameof(Done));
    }

    // STEP 4 — done
    [HttpGet]
    public IActionResult Done()
    {
        var applicantNumber = TempData["ApplicantNumber"] as string;
        if (string.IsNullOrEmpty(applicantNumber)) return RedirectToAction(nameof(Index));
        ViewData["ApplicantNumber"] = applicantNumber;
        return View();
    }
}