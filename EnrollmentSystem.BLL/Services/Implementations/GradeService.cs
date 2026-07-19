// File: EnrollmentSystem.BLL/Services/Implementations/GradeService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Models;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepo;

    public GradeService(IGradeRepository gradeRepo)
    {
        _gradeRepo = gradeRepo;
    }

    public async Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId)
        => await _gradeRepo.GetByStudentAsync(studentId);

    public async Task<IEnumerable<Grade>> GetByTeacherAsync(int teacherId)
        => await _gradeRepo.GetByTeacherAsync(teacherId);

    public async Task<ServiceResult> EncodeGradeAsync(Grade grade, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(grade.Quarter))
            return ServiceResult.Fail("Quarter is required.");

        if (grade.Grade1 is < 0 or > 100)
            return ServiceResult.Fail("Grade must be between 0 and 100.");

        var existing = await _gradeRepo.GetExistingAsync(grade.StudentId, grade.SubjectId, grade.Quarter);
        if (existing is not null)
        {
            existing.Grade1 = grade.Grade1;
            existing.GradeLevelId = grade.GradeLevelId;
            existing.TeacherId = grade.TeacherId;
            existing.ModifiedBy = createdBy;
            _gradeRepo.Update(existing);
        }
        else
        {
            grade.CreatedBy = createdBy;
            grade.IsDeleted = false;
            await _gradeRepo.AddAsync(grade);
        }

        await _gradeRepo.SaveAsync();
        return ServiceResult.Ok("Grade saved.");
    }

    public async Task<decimal?> GetGeneralAverageAsync(int studentId)
    {
        var grades = (await _gradeRepo.GetByStudentAsync(studentId))
            .Where(g => g.Grade1.HasValue)
            .Select(g => g.Grade1!.Value)
            .ToList();

        if (grades.Count == 0) return null;
        return Math.Round(grades.Average(), 2);
    }
    public async Task<IEnumerable<Grade>> GetBySubjectAndQuarterAsync(int subjectId, string quarter)
        => (await _gradeRepo.GetBySubjectAsync(subjectId))
            .Where(g => g.Quarter == quarter)
            .ToList();


}