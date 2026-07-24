// File: EnrollmentSystem.BLL/Services/Implementations/ReportingService.cs
using EnrollmentSystem.BLL.DTOs;
using EnrollmentSystem.BLL.Services.Interfaces;
using EnrollmentSystem.DAL.Repositories.Interfaces;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class ReportingService : IReportingService
{
    private readonly ISectionRepository _sectionRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly IGradeRepository _gradeRepo;
    private readonly IApplicationRepository _applicationRepo;
    private readonly IProofOfPaymentRepository _proofRepo;

    public ReportingService(
        ISectionRepository sectionRepo,
        IEnrollmentRepository enrollmentRepo,
        IStudentRepository studentRepo,
        IGradeRepository gradeRepo,
        IApplicationRepository applicationRepo,
        IProofOfPaymentRepository proofRepo)
    {
        _sectionRepo = sectionRepo;
        _enrollmentRepo = enrollmentRepo;
        _studentRepo = studentRepo;
        _gradeRepo = gradeRepo;
        _applicationRepo = applicationRepo;
        _proofRepo = proofRepo;
    }

    public async Task<IEnumerable<EnrollmentStatDto>> GetEnrollmentStatsAsync()
    {
        var sections = await _sectionRepo.GetAllActiveAsync();
        var stats = new List<EnrollmentStatDto>();

        foreach (var section in sections)
        {
            var enrollments = await _enrollmentRepo.GetBySectionAsync(section.SectionId);
            stats.Add(new EnrollmentStatDto
            {
                SectionName = section.Name,
                GradeLevel = section.GradeLevel?.Name ?? "-",
                SchoolYear = section.SchoolYear?.Name ?? "-",
                EnrolledCount = enrollments.Count(e => e.Status == "Enrolled")
            });
        }

        return stats;
    }

    public async Task<IEnumerable<ClassListRowDto>> GetClassListAsync(int sectionId)
    {
        var enrollments = (await _enrollmentRepo.GetBySectionAsync(sectionId))
            .Where(e => e.Status == "Enrolled")
            .ToList();

        var rows = new List<ClassListRowDto>();
        var i = 1;
        foreach (var e in enrollments.OrderBy(e => e.Student.LastName))
        {
            rows.Add(new ClassListRowDto
            {
                Number = i++,
                StudentId = e.StudentId,
                StudentNumber = e.Student.StudentNumber ?? "-",
                FullName = $"{e.Student.LastName}, {e.Student.FirstName} {e.Student.MiddleName}".Trim(),
                Gender = e.Student.Gender ?? "-",
                Status = e.Status
            });
        }

        return rows;
    }

    public async Task<ReportCardDto?> GetReportCardAsync(int studentId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student is null) return null;

        var grades = (await _gradeRepo.GetByStudentAsync(studentId)).ToList();

        var lines = grades
            .GroupBy(g => new { g.SubjectId, SubjectName = g.Subject?.Name ?? "-" })
            .Select(group =>
            {
                decimal? Q(string q) => group.FirstOrDefault(x => x.Quarter == q)?.Grade1;

                var q1 = Q("Q1"); var q2 = Q("Q2"); var q3 = Q("Q3"); var q4 = Q("Q4");
                var available = new[] { q1, q2, q3, q4 }.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                decimal? final = available.Count > 0 ? Math.Round(available.Average(), 2) : null;

                return new ReportCardLineDto
                {
                    Subject = group.Key.SubjectName,
                    Q1 = q1,
                    Q2 = q2,
                    Q3 = q3,
                    Q4 = q4,
                    Final = final
                };
            })
            .OrderBy(l => l.Subject)
            .ToList();

        var finals = lines.Where(l => l.Final.HasValue).Select(l => l.Final!.Value).ToList();

        return new ReportCardDto
        {
            StudentId = student.StudentId,
            StudentNumber = student.StudentNumber ?? "-",
            FullName = $"{student.LastName}, {student.FirstName} {student.MiddleName}".Trim(),
            Lines = lines,
            GeneralAverage = finals.Count > 0 ? Math.Round(finals.Average(), 2) : null
        };
    }

    public async Task<IEnumerable<MasterListRowDto>> GetMasterListAsync(int? schoolYearId)
    {
        var enrolled = (await _enrollmentRepo.GetByStatusAsync("Enrolled")).ToList();

        if (schoolYearId.HasValue)
            enrolled = enrolled.Where(e => e.Section?.SchoolYearId == schoolYearId.Value).ToList();

        var rows = new List<MasterListRowDto>();
        var i = 1;
        foreach (var e in enrolled.OrderBy(e => e.Student.LastName))
        {
            rows.Add(new MasterListRowDto
            {
                Number = i++,
                StudentNumber = e.Student.StudentNumber ?? "-",
                FullName = $"{e.Student.LastName}, {e.Student.FirstName} {e.Student.MiddleName}".Trim(),
                Section = e.Section?.Name ?? "-",
                GradeLevel = e.Section?.GradeLevel?.Name ?? "-",
                SchoolYear = e.Section?.SchoolYear?.Name ?? "-",
                Status = e.Status
            });
        }

        return rows;
    }
    public async Task<SectionGradeSheetDto> GetSectionGradesAsync(int sectionId)
    {
        var section = await _sectionRepo.GetWithSchedulesAsync(sectionId);
        var subjects = section?.Schedules
            .Where(sc => !sc.IsDeleted && sc.Subject is not null)
            .Select(sc => sc.Subject!)
            .GroupBy(x => x.SubjectId)
            .Select(g => g.First())
            .ToList() ?? new List<EnrollmentSystem.DAL.Models.Subject>();

        var dto = new SectionGradeSheetDto
        {
            SectionName = section?.Name ?? "-",
            Subjects = subjects.Select(s => s.Name).ToList()
        };

        var enrollments = (await _enrollmentRepo.GetBySectionAsync(sectionId))
            .Where(e => e.Status == "Enrolled")
            .OrderBy(e => e.Student.LastName)
            .ToList();

        foreach (var e in enrollments)
        {
            var grades = (await _gradeRepo.GetByStudentAsync(e.StudentId)).ToList();
            var row = new SectionGradeRowDto
            {
                StudentNumber = e.Student.StudentNumber ?? "-",
                StudentName = $"{e.Student.LastName}, {e.Student.FirstName}"
            };

            var finals = new List<decimal>();
            foreach (var subj in subjects)
            {
                var vals = grades.Where(g => g.SubjectId == subj.SubjectId && g.Grade1.HasValue)
                                 .Select(g => g.Grade1!.Value).ToList();
                decimal? final = vals.Count > 0 ? Math.Round(vals.Average(), 2) : null;
                row.Finals[subj.Name] = final;
                if (final.HasValue) finals.Add(final.Value);
            }
            row.GeneralAverage = finals.Count > 0 ? Math.Round(finals.Average(), 2) : null;
            dto.Rows.Add(row);
        }
        return dto;
    }
    public async Task<DashboardChartsDto> GetDashboardChartsAsync()
    {
        var dto = new DashboardChartsDto();

        // Enrolled students per grade level
        var sections = await _sectionRepo.GetAllActiveAsync();
        var byGrade = new Dictionary<string, int>();
        foreach (var s in sections)
        {
            var enrolled = (await _enrollmentRepo.GetBySectionAsync(s.SectionId)).Count(e => e.Status == "Enrolled");
            var g = s.GradeLevel?.Name ?? s.Name;
            byGrade[g] = byGrade.GetValueOrDefault(g) + enrolled;
        }
        dto.GradeLabels = byGrade.Keys.ToList();
        dto.GradeEnrollment = byGrade.Values.ToList();

        // Applications by status
        var apps = await _applicationRepo.GetAllActiveAsync();
        foreach (var grp in apps.GroupBy(a => a.Status))
        {
            dto.AppStatusLabels.Add(grp.Key);
            dto.AppStatusCounts.Add(grp.Count());
        }

        // Grade distribution
        var grades = (await _gradeRepo.GetAllActiveAsync())
            .Where(g => g.Grade1.HasValue).Select(g => g.Grade1!.Value).ToList();
        var buckets = new (string label, Func<decimal, bool> test)[]
        {
            ("90–100", v => v >= 90),
            ("85–89",  v => v >= 85 && v < 90),
            ("80–84",  v => v >= 80 && v < 85),
            ("75–79",  v => v >= 75 && v < 80),
            ("Below 75", v => v < 75),
        };
        foreach (var b in buckets)
        {
            dto.GradeDistLabels.Add(b.label);
            dto.GradeDistCounts.Add(grades.Count(b.test));
        }

        // Payments by status
        var payments = await _proofRepo.GetAllWithDetailsAsync();
        dto.PaymentLabels = new List<string> { "Verified", "Pending", "Rejected" };
        dto.PaymentCounts = new List<int>
        {
            payments.Count(p => p.Status == "Verified"),
            payments.Count(p => p.Status == "Pending"),
            payments.Count(p => p.Status == "Rejected")
        };

        return dto;
    }
}