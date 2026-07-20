// File: EnrollmentSystem.UI/Controllers/AccountController.cs
using System.Security.Claims;
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.UI.Helpers;
using EnrollmentSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Dashboard", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(new LoginDto
        {
            UserName = model.UserName,
            Password = model.Password
        });

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Login failed.");
            return View(model);
        }

        var user = result.Data;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("DisplayName", BuildDisplayName(user)),
            new("MustChangePassword", user.MustChangePassword ? "true" : "false")
        };
        foreach (var r in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        if (user.MustChangePassword)
            return RedirectToAction(nameof(ChangePassword));

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Dashboard", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        // Public sign-ups now go through the Apply Now wizard (email verification).
        return RedirectToAction("Index", "Apply");
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction(nameof(Login));

        var result = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Could not change password.");
            return View(model);
        }

        // Sign out so the stale MustChangePassword claim is discarded; user signs in fresh.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Password changed. Please sign in with your new password.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        // Logged-in users are sent to their own dashboard instead of seeing the denied page.
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Dashboard", "Home");

        return View();
    }

    private static string BuildDisplayName(AuthUserDto user)
    {
        if (user.Roles.Contains("Admin"))
            return "Administrator";

        if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains('@'))
        {
            var local = user.Email[..user.Email.IndexOf('@')];
            if (local.Length > 0)
                return char.ToUpper(local[0]) + local[1..];
        }

        return user.UserName;
    }
}