using LogicPOS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// LogicPulse software-producer admin entry. Works on any database without a matching user row.
/// Credential check uses a verifier from configuration (PBKDF2, fail-closed); see
/// <see cref="SoftwareProducerServiceCollectionExtensions.AddSoftwareProducerAuth"/>.
/// </summary>
public static class SoftwareProducerAuth
{
    /// <summary>Stable identity used in JWTs and permission lookups.</summary>
    public static readonly Guid UserId = new("a1000000-10c1-4000-8000-000000000001");

    public const string Login = "logicpulse";
    public const string DisplayName = "LogicPulse";

    private static ISoftwareProducerPasswordVerifier? _verifier;
    private static ILogger? _logger;
    private static int _notConfiguredLogged;

    /// <summary>
    /// Wires the password verifier used by <see cref="Matches"/>. Prefer
    /// <see cref="SoftwareProducerServiceCollectionExtensions.AddSoftwareProducerAuth"/> /
    /// <see cref="SoftwareProducerServiceCollectionExtensions.UseSoftwareProducerAuth"/>.
    /// </summary>
    public static void Configure(ISoftwareProducerPasswordVerifier verifier, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(verifier);
        _verifier = verifier;
        _logger = logger;
        Interlocked.Exchange(ref _notConfiguredLogged, 0);
    }

    /// <summary>Clears the configured verifier (tests).</summary>
    internal static void ResetConfiguration()
    {
        _verifier = null;
        _logger = null;
        Interlocked.Exchange(ref _notConfiguredLogged, 0);
    }

    /// <summary>
    /// Returns true when login is the producer login and the password matches the configured verifier.
    /// Fail-closed when no verifier has been configured.
    /// </summary>
    public static bool Matches(string? login, string? password) =>
        string.Equals(login?.Trim(), Login, StringComparison.OrdinalIgnoreCase)
        && string.IsNullOrEmpty(password) == false
        && VerifyConfigured(password);

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

    private static bool VerifyConfigured(string password)
    {
        var verifier = _verifier;
        if (verifier is null)
        {
            if (Interlocked.Exchange(ref _notConfiguredLogged, 1) == 0)
            {
                _logger?.LogWarning("SoftwareProducer verifier is not configured; producer login is refused.");
            }

            return false;
        }

        return verifier.VerifyPassword(password);
    }
}
