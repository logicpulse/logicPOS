using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Fail-closed SoftwareProducer password check against <see cref="SoftwareProducerOptions"/>.
/// </summary>
public sealed class SoftwareProducerPasswordVerifier : ISoftwareProducerPasswordVerifier
{
    private readonly IOptionsMonitor<SoftwareProducerOptions> _options;
    private readonly ILogger<SoftwareProducerPasswordVerifier> _logger;
    private int _notConfiguredLogged;

    public SoftwareProducerPasswordVerifier(
        IOptionsMonitor<SoftwareProducerOptions> options,
        ILogger<SoftwareProducerPasswordVerifier> logger)
    {
        _options = options;
        _logger = logger;
    }

    public bool VerifyPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        var options = _options.CurrentValue;
        var hasVerifier = !string.IsNullOrWhiteSpace(options.Verifier);
        var legacyEnabled = options.AcceptLegacy && !string.IsNullOrWhiteSpace(options.LegacyVerifier);

        if (!hasVerifier && !legacyEnabled)
        {
            if (Interlocked.Exchange(ref _notConfiguredLogged, 1) == 0)
            {
                _logger.LogWarning("SoftwareProducer verifier is not configured; producer login is refused.");
            }

            return false;
        }

        if (hasVerifier
            && SoftwareProducerPbkdf2.TryVerify(password, options.Verifier, _logger))
        {
            return true;
        }

        if (legacyEnabled)
        {
            return LegacyPasswordHasherFormat.VerifyPassword(password, options.LegacyVerifier!);
        }

        return false;
    }
}
