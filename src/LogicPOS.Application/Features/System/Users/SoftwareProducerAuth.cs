using System.Security.Cryptography;
using System.Text;
using LogicPOS.Domain.Entities;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// LogicPulse software-producer admin entry. Works on any database without a matching user row.
/// Credential check uses a fixed PasswordHasher-format verifier (not a tenant user password).
/// </summary>
public static class SoftwareProducerAuth
{
    /// <summary>Stable identity used in JWTs and permission lookups.</summary>
    public static readonly Guid UserId = new("a1000000-10c1-4000-8000-000000000001");

    public const string Login = "logicpulse";
    public const string DisplayName = "LogicPulse";

    /// <summary>
    /// Fixed salt*SHA512 verifier matching <c>PasswordHasher</c> format for the producer password.
    /// </summary>
    private const string PasswordVerifier =
        "LpPr0d*YQa9NhfaYQfMB4V7Sc+8aQpkb6gW+Ay4EMXBNKV+12rEhpScrpwNsN4vGDw1COqdMJ4piU9PE3a33t3yqUEQSQ==";

    public static bool Matches(string? login, string? password) =>
        string.Equals(login?.Trim(), Login, StringComparison.OrdinalIgnoreCase)
        && string.IsNullOrEmpty(password) == false
        && VerifyPassword(password, PasswordVerifier);

    public static bool IsProducerUser(Guid userId) => userId == UserId;

    public static User CreateVirtualUser(Guid? profileId = null) =>
        new()
        {
            Id = UserId,
            Login = Login,
            Name = DisplayName,
            Code = "0",
            Order = 0,
            AccessPin = string.Empty,
            ProfileId = profileId ?? Guid.Empty,
            PasswordReset = false,
            Need2FA = false,
            IsDeleted = false
        };

    private static bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split('*');
        if (parts.Length != 2)
            return false;

        var salt = parts[0];
        var actual = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(salt + password)));
        return string.Equals(actual, parts[1], StringComparison.Ordinal);
    }
}
