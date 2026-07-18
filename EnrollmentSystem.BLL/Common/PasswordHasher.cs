// File: EnrollmentSystem.BLL/Common/PasswordHasher.cs
using System.Security.Cryptography;

namespace EnrollmentSystem.BLL.Common;

/// <summary>
/// PBKDF2 (SHA-256) password hashing. Stored format: "iterations.salt.hash" (base64).
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash)) return false;

        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out var iterations)) return false;

        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] key = Convert.FromBase64String(parts[2]);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, key.Length);

        return CryptographicOperations.FixedTimeEquals(hash, key);
    }
}