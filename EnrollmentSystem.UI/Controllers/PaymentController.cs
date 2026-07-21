// File: EnrollmentSystem.UI/Controllers/PaymentController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class PaymentController : Controller
{
    private readonly IPaymentService _payments;

    public PaymentController(IPaymentService payments)
    {
        _payments = payments;
    }

    public async Task<IActionResult> Index(string status = "All")
    {
        ViewData["Status"] = status;
        return View(await _payments.GetAllAsync(status));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int proofId, string status, string? remarks)
    {
        var result = await _payments.ReviewAsync(proofId, status, remarks, User.GetUserName());
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}