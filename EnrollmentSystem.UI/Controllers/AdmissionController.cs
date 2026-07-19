// File: EnrollmentSystem.UI/Controllers/AdmissionController.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class AdmissionController : Controller
{
    private readonly IAdmissionService _admission;
    private readonly IApplicationService _applicationService;
    private readonly IProofOfPaymentService _proofService;
    private readonly IStudentService _studentService;

    public AdmissionController(
        IAdmissionService admission,
        IApplicationService applicationService,
        IProofOfPaymentService proofService,
        IStudentService studentService)
    {
        _admission = admission;
        _applicationService = applicationService;
        _proofService = proofService;
        _studentService = studentService;
    }

    public async Task<IActionResult> Index(string status = "All")
    {
        ViewData["Status"] = status;
        var apps = await _admission.GetQueueAsync(status);
        return View(apps);
    }

    public async Task<IActionResult> Review(int id)
    {
        var app = await _admission.GetFullAsync(id);
        if (app is null) return NotFound();

        ViewBag.Documents = await _applicationService.GetDocumentsAsync(id);
        ViewBag.Payments = await _proofService.GetByApplicationAsync(id);
        return View(app);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, string status, string? remarks)
    {
        var result = await _admission.ReviewAsync(id, status, remarks, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Review), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyPayment(int proofId, int applicationId, string status, string? remarks)
    {
        var result = await _admission.VerifyPaymentAsync(proofId, status, remarks, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Review), new { id = applicationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalize(int id)
    {
        var result = await _studentService.FinalizeEnrollmentAsync(id, User.GetUserName());

        if (result.Success && result.Data is not null)
        {
            TempData["Success"] = result.Message;
            TempData["NewStudentNumber"] = result.Data.StudentNumber;
            TempData["NewStudentTempPassword"] = result.Data.TempPassword;
        }
        else
        {
            TempData["Error"] = result.Message;
        }
        return RedirectToAction(nameof(Review), new { id });
    }
}