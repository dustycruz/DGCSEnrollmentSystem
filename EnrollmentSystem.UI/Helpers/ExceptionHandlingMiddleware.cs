// File: EnrollmentSystem.UI/Middleware/ExceptionHandlingMiddleware.cs
namespace EnrollmentSystem.UI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

            // If the response already started, or we're already on the error page,
            // never redirect again — that would cause an infinite redirect loop.
            if (context.Response.HasStarted ||
                context.Request.Path.StartsWithSegments("/Home/Error"))
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("An unexpected error occurred.");
                }
                return;
            }

            // Pass a short reason to the error page so it can be seen without digging in logs.
            var reason = Uri.EscapeDataString($"{ex.GetType().Name}: {ex.Message}");
            context.Response.Redirect("/Home/Error?reason=" + reason);
        }
    }
}