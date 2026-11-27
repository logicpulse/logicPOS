using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Fail-closed SoftwareProducer password check against <see cref="SoftwareProducerOptions"/>.
/// </summary>
public sealed class SoftwareProducerPasswordVerifier : ISoftwareProducerPasswordVerifier
{
    private static int _notConfiguredLogged;

    private readonly IOptionsMonitor<SoftwareProducerOptions> _options;
    private readonly ILogger<SoftwareProducerPasswordVerifier> _logger;

    public SoftwareProducerPasswordVerifier(
        IOptionsMonitor<SoftwareProducerOptions> options,
        ILogger<SoftwareProducerPasswordVerifier> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <summary>Resets the once-per-process "not configured" warning flag (tests).</summary>
    internal static void ResetNotConfiguredLogForTests() =>
        Interlocked.Exchange(ref _notConfiguredLogged, 0);

    public bool VerifyPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        var options = _options.CurrentValue;
        if (string.IsNullOrWhiteSpace(options.Verifier))
        {
            if (Interlocked.Exchange(ref _notConfiguredLogged, 1) == 0)
            {
                _logger.LogWarning("SoftwareProducer verifier is not configured; producer login is refused.");
            }

            return false;
        }

        return SoftwareProducerPbkdf2.TryVerify(password, options.Verifier, _logger);
    }
}
