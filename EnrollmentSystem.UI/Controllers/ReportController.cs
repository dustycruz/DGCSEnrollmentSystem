// File: EnrollmentSystem.UI/Controllers/ReportController.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentSystem.UI.Controllers;

[Authorize(Roles = "Admin")]
public class ReportController : Controller
{
    private readonly IReportingService _reporting;
    private readonly ISectionService _sectionService;
    private readonly IStudentService _studentService;
    private readonly ILookupService _lookup;

    public ReportController(
        IReportingService reporting,
        ISectionService sectionService,
        IStudentService studentService,
        ILookupService lookup)
    {
        _reporting = reporting;
        _sectionService = sectionService;
        _studentService = studentService;
        _lookup = lookup;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> EnrollmentStats()
        => View(await _reporting.GetEnrollmentStatsAsync());

    public async Task<IActionResult> ClassList(int? sectionId)
    {
        ViewBag.Sections = (await _sectionService.GetAllAsync())
            .Select(s => new SelectListItem(s.Name, s.SectionId.ToString(), s.SectionId == sectionId));
        var rows = sectionId.HasValue ? await _reporting.GetClassListAsync(sectionId.Value) : new List<EnrollmentSystem.BLL.DTOs.ClassListRowDto>();
        ViewBag.SectionId = sectionId;
        return View(rows);
    }

    public async Task<IActionResult> MasterList(int? schoolYearId)
    {
        ViewBag.SchoolYears = (await _lookup.GetSchoolYearsAsync())
            .Select(y => new SelectListItem(y.Name, y.SchoolYearId.ToString(), y.SchoolYearId == schoolYearId));
        return View(await _reporting.GetMasterListAsync(schoolYearId));
    }

    public async Task<IActionResult> ReportCard(int? studentId)
    {
        ViewBag.Students = (await _studentService.GetAllAsync())
            .Select(s => new SelectListItem($"{s.LastName}, {s.FirstName} ({s.StudentNumber})", s.StudentId.ToString(), s.StudentId == studentId));
        var card = studentId.HasValue ? await _reporting.GetReportCardAsync(studentId.Value) : null;
        return View(card);
    }
}