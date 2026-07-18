// File: EnrollmentSystem.BLL/Services/Interfaces/IFileStorageService.cs
using EnrollmentSystem.BLL.Common;
using Microsoft.AspNetCore.Http;

namespace EnrollmentSystem.BLL.Services.Interfaces;

public interface IFileStorageService
{
    Task<ServiceResult<string>> SaveFileAsync(IFormFile file, string subFolder);
    void DeleteFile(string relativePath);
}