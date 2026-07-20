// File: EnrollmentSystem.UI/Controllers/AdminController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAuthService _authService;

    public AdminController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Index() => RedirectToAction(nameof(Users));

    public async Task<IActionResult> Users()
    {
        var users = await _authService.GetAllUsersWithRolesAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        var result = await _authService.ResetPasswordAsync(userId);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            TempData["ResetUserId"] = userId;
            TempData["ResetTempPassword"] = result.Data;
        }
        else
        {
            TempData["Error"] = result.Message;
        }
        return RedirectToAction(nameof(Users));
    }
}