// File: EnrollmentSystem.UI/Middleware/PasswordChangeMiddleware.cs
namespace EnrollmentSystem.UI.Middleware;

public class PasswordChangeMiddleware
{
    private readonly RequestDelegate _next;

    public PasswordChangeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated == true &&
            user.FindFirst("MustChangePassword")?.Value == "true")
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            var allowed =
                path.StartsWith("/account/changepassword") ||
                path.StartsWith("/account/logout") ||
                path.StartsWith("/css") || path.StartsWith("/js") ||
                path.StartsWith("/lib") || path.StartsWith("/images");

            if (!allowed)
            {
                context.Response.Redirect("/Account/ChangePassword");
                return;
            }
        }

        await _next(context);
    }
}