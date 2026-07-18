// File: EnrollmentSystem.BLL/Services/Implementations/FileStorageService.cs
using EnrollmentSystem.BLL.Common;
using EnrollmentSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EnrollmentSystem.BLL.Services.Implementations;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
    private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<ServiceResult<string>> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file is null || file.Length == 0)
            return ServiceResult<string>.Fail("No file was selected.");

        if (file.Length > MaxBytes)
            return ServiceResult<string>.Fail("File exceeds the 5 MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return ServiceResult<string>.Fail("Invalid file type. Allowed: JPG, PNG, WEBP, PDF.");

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var targetFolder = Path.Combine(webRoot, "uploads", subFolder);
        Directory.CreateDirectory(targetFolder);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetFolder, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/{subFolder}/{fileName}";
        return ServiceResult<string>.Ok(relativePath, "File uploaded successfully.");
    }

    public void DeleteFile(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var trimmed = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(webRoot, trimmed);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}