// File: EnrollmentSystem.UI/Controllers/ApplicantController.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Applicant")]
public class ApplicantController : Controller
{
    private readonly IApplicationService _applicationService;
    private readonly IProofOfPaymentService _proofService;
    private readonly IFileStorageService _fileStorage;
    private readonly ILookupService _lookup;
    private readonly PaymentInstructionSettings _paymentInfo;

    public ApplicantController(
        IApplicationService applicationService,
        IProofOfPaymentService proofService,
        IFileStorageService fileStorage,
        ILookupService lookup,
        IOptions<PaymentInstructionSettings> paymentInfo)
    {
        _applicationService = applicationService;
        _proofService = proofService;
        _fileStorage = fileStorage;
        _lookup = lookup;
        _paymentInfo = paymentInfo.Value;
    }

    private string Me => User.GetUserName();

    private async Task<Application?> MyApplicationAsync()
        => (await _applicationService.GetMyApplicationsAsync(Me)).FirstOrDefault();

    // ---------- Dashboard / status tracker ----------
    public async Task<IActionResult> Index()
    {
        var app = await MyApplicationAsync();
        var vm = new ApplicantPortalViewModel { Application = app, PaymentInfo = _paymentInfo };

        if (app is not null)
        {
            vm.RequiredDocuments = await _lookup.GetAdmissionDocumentsAsync();
            vm.UploadedDocuments = await _applicationService.GetDocumentsAsync(app.ApplicationId);
            vm.Payments = await _proofService.GetByApplicationAsync(app.ApplicationId);
        }
        return View(vm);
    }

    // ---------- Admission form ----------
    [HttpGet]
    public async Task<IActionResult> Apply()
    {
        var app = await MyApplicationAsync();
        var vm = app is null
            ? new AdmissionFormViewModel { EmailAddress = User.GetEmail() }
            : new AdmissionFormViewModel
            {
                ApplicationId = app.ApplicationId,
                FirstName = app.FirstName,
                MiddleName = app.MiddleName,
                LastName = app.LastName,
                Birthday = app.Birthday,
                Gender = app.Gender,
                PlaceOfBirth = app.PlaceOfBirth,
                Religion = app.Religion,
                Address = app.Address,
                MobileNumber = app.MobileNumber,
                EmailAddress = app.EmailAddress ?? User.GetEmail(),
                HomeNumber = app.HomeNumber,
                LearnerReferenceNumber = app.LearnerReferenceNumber,
                LastSchoolAttended = app.LastSchoolAttended,
                LastLevelObtained = app.LastLevelObtained,
                GuardianName = app.GuardianName,
                GuardianContactNumber = app.GuardianContactNumber,
                GradeLevelId = app.GradeLevelId,
                SchoolYearId = app.SchoolYearId,
                ApplicantType = app.ApplicantType ?? "New Student"
            };

        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(AdmissionFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        var app = new Application
        {
            ApplicationId = vm.ApplicationId ?? 0,
            FirstName = vm.FirstName,
            MiddleName = vm.MiddleName,
            LastName = vm.LastName,
            Birthday = vm.Birthday,
            Gender = vm.Gender,
            PlaceOfBirth = vm.PlaceOfBirth,
            Religion = vm.Religion,
            Address = vm.Address,
            MobileNumber = vm.MobileNumber,
            EmailAddress = string.IsNullOrWhiteSpace(vm.EmailAddress) ? User.GetEmail() : vm.EmailAddress,
            HomeNumber = vm.HomeNumber,
            LearnerReferenceNumber = vm.LearnerReferenceNumber,
            LastSchoolAttended = vm.LastSchoolAttended,
            LastLevelObtained = vm.LastLevelObtained,
            GuardianName = vm.GuardianName,
            GuardianContactNumber = vm.GuardianContactNumber,
            GradeLevelId = vm.GradeLevelId,
            SchoolYearId = vm.SchoolYearId,
            ApplicantType = vm.ApplicantType
        };

        if (vm.ApplicationId is null or 0)
        {
            var result = await _applicationService.SubmitApplicationAsync(app, Me);
            TempData["Success"] = result.Message;
        }
        else
        {
            var result = await _applicationService.UpdateApplicationAsync(app, Me);
            TempData["Success"] = result.Message;
        }

        return RedirectToAction(nameof(Requirements));
    }

    // ---------- Requirements upload ----------
    [HttpGet]
    public async Task<IActionResult> Requirements()
    {
        var app = await MyApplicationAsync();
        if (app is null) return RedirectToAction(nameof(Apply));

        var vm = new ApplicantPortalViewModel
        {
            Application = app,
            RequiredDocuments = await _lookup.GetAdmissionDocumentsAsync(),
            UploadedDocuments = await _applicationService.GetDocumentsAsync(app.ApplicationId)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadRequirement(int documentId, IFormFile file)
    {
        var app = await MyApplicationAsync();
        if (app is null) return RedirectToAction(nameof(Apply));

        var saved = await _fileStorage.SaveFileAsync(file, "requirements");
        if (!saved.Success)
        {
            TempData["Error"] = saved.Message;
            return RedirectToAction(nameof(Requirements));
        }

        await _applicationService.AddDocumentAsync(app.ApplicationId, saved.Data!, documentId);
        TempData["Success"] = "Requirement uploaded.";
        return RedirectToAction(nameof(Requirements));
    }

    // ---------- Payment ----------
    [HttpGet]
    public async Task<IActionResult> Payment()
    {
        var app = await MyApplicationAsync();
        if (app is null) return RedirectToAction(nameof(Apply));

        var vm = new ApplicantPortalViewModel
        {
            Application = app,
            PaymentInfo = _paymentInfo,
            Payments = await _proofService.GetByApplicationAsync(app.ApplicationId)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPayment(string referenceNumber, decimal? amountPaid,
        string? paymentMethod, string purpose, IFormFile file)
    {
        var app = await MyApplicationAsync();
        if (app is null) return RedirectToAction(nameof(Apply));

        var saved = await _fileStorage.SaveFileAsync(file, "payments");
        if (!saved.Success)
        {
            TempData["Error"] = saved.Message;
            return RedirectToAction(nameof(Payment));
        }

        var proof = new ProofOfPayment
        {
            ApplicationId = app.ApplicationId,
            ReferenceNumber = referenceNumber,
            AmountPaid = amountPaid,
            PaymentMethod = paymentMethod,
            Purpose = string.IsNullOrWhiteSpace(purpose) ? PaymentPurposes.ApplicationFee : purpose,
            FilePath = saved.Data,
            PaymentDate = DateTime.Now
        };

        await _proofService.SubmitAsync(proof, Me);
        TempData["Success"] = "Proof of payment submitted for verification.";
        return RedirectToAction(nameof(Payment));
    }

    private async Task PopulateDropdownsAsync(AdmissionFormViewModel vm)
    {
        vm.GradeLevels = (await _lookup.GetGradeLevelsAsync())
            .Select(g => new SelectListItem(g.Name, g.GradeLevelId.ToString()));
        vm.SchoolYears = (await _lookup.GetSchoolYearsAsync())
            .Select(y => new SelectListItem(y.Name, y.SchoolYearId.ToString()));
    }
}