// File: EnrollmentSystem.BLL/Services/Interfaces/ILookupService.cs
using EnrollmentSystem.DAL.Models;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface ILookupService
{
    Task<IEnumerable<SchoolYear>> GetSchoolYearsAsync();
    Task<IEnumerable<GradeLevel>> GetGradeLevelsAsync();
    Task<IEnumerable<EducationalLevel>> GetEducationalLevelsAsync();
    Task<IEnumerable<Section>> GetSectionsAsync();
    Task<IEnumerable<Subject>> GetSubjectsAsync();
    Task<IEnumerable<Curriculum>> GetCurriculaAsync();
    Task<IEnumerable<Teacher>> GetTeachersAsync();
}