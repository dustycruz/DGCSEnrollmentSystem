// File: EnrollmentSystem.UI/Controllers/SettingsController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly IAuthService _authService;

    public SettingsController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Index()
    {
        ViewBag.UserName = User.GetUserName();
        ViewBag.Email = User.GetEmail();
        ViewBag.Role = User.GetPrimaryRole();
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserName = User.GetUserName();
            ViewBag.Email = User.GetEmail();
            ViewBag.Role = User.GetPrimaryRole();
            return View(nameof(Index), model);
        }

        var userId = User.GetUserId();
        var result = await _authService.ChangePasswordAsync(userId!, model.CurrentPassword, model.NewPassword);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}