// File: EnrollmentSystem.BLL/Common/CredentialGenerator.cs
using System.Security.Cryptography;

namespace EnrollmentSystem.BLL.Common;

public static class CredentialGenerator
{
    /// <summary>6-digit OTP, e.g. "482913".</summary>
    public static string GenerateOtp()
        => RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

    /// <summary>Readable temporary password, e.g. "DGCS-x7K2qP9m".</summary>
    public static string GenerateTempPassword()
    {
        const string chars = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
        Span<char> buffer = stackalloc char[8];
        for (var i = 0; i < buffer.Length; i++)
            buffer[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        return $"DGCS-{new string(buffer)}";
    }
}