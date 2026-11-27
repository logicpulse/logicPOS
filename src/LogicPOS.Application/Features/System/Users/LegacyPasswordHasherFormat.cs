using System.Security.Cryptography;
using System.Text;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Legacy PasswordHasher format (<c>salt*SHA512-base64</c>) used by tenant user passwords
/// and optional SoftwareProducer legacy verifiers. Shared so auth does not reimplement the algorithm.
/// </summary>
public static class LegacyPasswordHasherFormat
{
    private const string SaltDelimiter = "*";
    private const int SaltLength = 6;

    /// <summary>Creates a legacy verifier (for tests / migration tooling only).</summary>
    public static string HashPassword(string password)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(SaltLength));
        return salt + SaltDelimiter + HashPasswordWithSalt(password, salt);
    }

    /// <summary>Verifies a password against a legacy <c>salt*digest</c> string using fixed-time comparison of digests when possible.</summary>
    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        var parts = hash.Split(SaltDelimiter);
        if (parts.Length != 2)
            return false;

        var salt = parts[0];
        var expectedBase64 = parts[1];
        var actualBase64 = HashPasswordWithSalt(password, salt);

        try
        {
            var expected = Convert.FromBase64String(expectedBase64);
            var actual = Convert.FromBase64String(actualBase64);
            if (expected.Length != actual.Length)
                return false;
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return string.Equals(actualBase64, expectedBase64, StringComparison.Ordinal);
        }
    }

    private static string HashPasswordWithSalt(string password, string salt)
    {
        var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToBase64String(bytes);
    }
}
