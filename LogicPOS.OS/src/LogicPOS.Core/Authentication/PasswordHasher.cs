using System.Security.Cryptography;
using System.Text;
using LogicPOS.Domain.Services;

namespace LogicPOS.Core.Authentication;

/// <summary>
/// Matches the PIN hash stored by the API (salt, asterisk, SHA-512) so existing users can sign in.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltLength = 6;
    private const string SaltDelimiter = "*";

    public string HashPassword(string password)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(SaltLength));
        var hash = HashPasswordWithSalt(password, salt);
        return salt + SaltDelimiter + hash;
    }

    public bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split(SaltDelimiter);

        if (parts.Length != 2)
        {
            return false;
        }

        var salt = parts[0];
        var pinHashWithSalt = HashPasswordWithSalt(password, salt);
        return pinHashWithSalt == parts[1];
    }

    private static string HashPasswordWithSalt(string pin, string salt)
    {
        var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(salt + pin));
        return Convert.ToBase64String(bytes);
    }
}
