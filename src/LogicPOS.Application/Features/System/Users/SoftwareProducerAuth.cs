using LogicPOS.Domain.Entities;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// LogicPulse software-producer admin entry. Works on any database without a matching user row.
/// </summary>
public static class SoftwareProducerAuth
{
    /// <summary>Stable identity used in JWTs and permission lookups.</summary>
    public static readonly Guid UserId = new("a1000000-10c1-4000-8000-000000000001");

    public const string Login = "logicpulse";
    public const string Password = "admin#";
    public const string DisplayName = "LogicPulse";

    public static bool Matches(string? login, string? password) =>
        string.Equals(login?.Trim(), Login, StringComparison.OrdinalIgnoreCase)
        && string.Equals(password, Password, StringComparison.Ordinal);

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
}
