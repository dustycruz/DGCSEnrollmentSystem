// File: EnrollmentSystem.BLL/Services/Implementations/LookupService.cs
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Implementations;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class LookupService : ILookupService
{
    private readonly ISchoolYearRepository _schoolYearRepo;
    private readonly IGradeLevelRepository _gradeLevelRepo;
    private readonly IEducationalLevelRepository _educationalLevelRepo;
    private readonly ISectionRepository _sectionRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly ICurriculumRepository _curriculumRepo;
    private readonly ITeacherRepository _teacherRepo;

    public LookupService(
        ISchoolYearRepository schoolYearRepo,
        IGradeLevelRepository gradeLevelRepo,
        IEducationalLevelRepository educationalLevelRepo,
        ISectionRepository sectionRepo,
        ISubjectRepository subjectRepo,
        ICurriculumRepository curriculumRepo,
        ITeacherRepository teacherRepo)
    {
        _schoolYearRepo = schoolYearRepo;
        _gradeLevelRepo = gradeLevelRepo;
        _educationalLevelRepo = educationalLevelRepo;
        _sectionRepo = sectionRepo;
        _subjectRepo = subjectRepo;
        _curriculumRepo = curriculumRepo;
        _teacherRepo = teacherRepo;
    }

    public async Task<IEnumerable<SchoolYear>> GetSchoolYearsAsync() => await _schoolYearRepo.GetAllActiveAsync();
    public async Task<IEnumerable<GradeLevel>> GetGradeLevelsAsync() => await _gradeLevelRepo.GetAllActiveAsync();
    public async Task<IEnumerable<EducationalLevel>> GetEducationalLevelsAsync() => await _educationalLevelRepo.GetAllActiveAsync();
    public async Task<IEnumerable<Section>> GetSectionsAsync() => await _sectionRepo.GetAllActiveAsync();
    public async Task<IEnumerable<Subject>> GetSubjectsAsync() => await _subjectRepo.GetAllActiveAsync();
    public async Task<IEnumerable<Curriculum>> GetCurriculaAsync() => await _curriculumRepo.GetAllActiveAsync();
    public async Task<IEnumerable<Teacher>> GetTeachersAsync() => await _teacherRepo.GetAllWithEmployeeAsync();
}