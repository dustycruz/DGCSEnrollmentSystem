// File: EnrollmentSystem.UI/Controllers/AuditController.cs
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class AuditController : Controller
{
    private readonly IAuditService _audit;

    public AuditController(IAuditService audit)
    {
        _audit = audit;
    }

    // GET: /Audit
    // NOTE: the action filter is bound from "act" (not "action") because "action"
    // is a reserved MVC routing token and would break URL generation for the links.
    public async Task<IActionResult> Index(
        string? act,
        string? entityName,
        string? keyword,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1)
    {
        var filter = new AuditLogFilterDto
        {
            Action = act,
            EntityName = entityName,
            Keyword = keyword,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page < 1 ? 1 : page,
            PageSize = 25
        };

        var model = await _audit.GetLogsAsync(filter);
        return View(model);
    }
}