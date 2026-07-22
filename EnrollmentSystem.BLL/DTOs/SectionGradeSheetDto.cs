// File: EnrollmentSystem.BLL/DTOs/SectionGradeSheetDto.cs
namespace EnrollmentSystem.BLL.DTOs;

public class SectionGradeRowDto
{
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public Dictionary<string, decimal?> Finals { get; set; } = new();
    public decimal? GeneralAverage { get; set; }
}

public class SectionGradeSheetDto
{
    public string SectionName { get; set; } = string.Empty;
    public List<string> Subjects { get; set; } = new();
    public List<SectionGradeRowDto> Rows { get; set; } = new();
}