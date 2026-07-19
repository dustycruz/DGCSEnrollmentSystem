// File: EnrollmentSystem.UI/Helpers/ClaimsPrincipalExtensions.cs
using System.Security.Claims;

namespace EnrollmentSystem.UI.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static string GetUserName(this ClaimsPrincipal user)
        => user.Identity?.Name ?? "User";

    public static string? GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Email)?.Value;

    public static string? GetPrimaryRole(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Role)?.Value;

    /// <summary>Friendly name for greetings (falls back to the login username).</summary>
    public static string GetDisplayName(this ClaimsPrincipal user)
        => user.FindFirst("DisplayName")?.Value ?? user.GetUserName();

    public static string GetInitials(this ClaimsPrincipal user)
    {
        var name = user.GetDisplayName();
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "U";
        if (parts.Length == 1) return parts[0][..1].ToUpperInvariant();
        return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
    }
}