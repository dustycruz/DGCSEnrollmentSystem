// File: EnrollmentSystem.BLL/Services/Interfaces/IReportingService.cs
using EnrollmentSystem.BLL.DTOs;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IReportingService
{
    Task<IEnumerable<EnrollmentStatDto>> GetEnrollmentStatsAsync();
    Task<IEnumerable<ClassListRowDto>> GetClassListAsync(int sectionId);
    Task<ReportCardDto?> GetReportCardAsync(int studentId);
    Task<IEnumerable<MasterListRowDto>> GetMasterListAsync(int? schoolYearId);
}