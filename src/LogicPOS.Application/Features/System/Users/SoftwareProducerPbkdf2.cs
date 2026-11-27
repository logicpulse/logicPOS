using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// PBKDF2-SHA256 verifier format: <c>pbkdf2-sha256$&lt;iterations&gt;$&lt;base64 salt&gt;$&lt;base64 hash&gt;</c>.
/// </summary>
public static class SoftwareProducerPbkdf2
{
    public const string AlgorithmId = "pbkdf2-sha256";
    public const int DefaultIterations = 600_000;
    public const int MinimumIterations = 600_000;
    public const int SaltSizeBytes = 16;
    public const int HashSizeBytes = 32;

    /// <summary>
    /// Creates a new verifier for <paramref name="password"/>. Salt is 16+ random bytes from
    /// <see cref="RandomNumberGenerator"/>; hash is 32 bytes via <see cref="Rfc2898DeriveBytes.Pbkdf2"/>.
    /// </summary>
    public static string CreateVerifier(string password, int iterations = DefaultIterations)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (iterations < MinimumIterations)
            throw new ArgumentOutOfRangeException(nameof(iterations), iterations, $"Iterations must be at least {MinimumIterations}.");

        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            HashSizeBytes);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{AlgorithmId}${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}");
    }

    /// <summary>
    /// Verifies <paramref name="password"/> against a PBKDF2 verifier string.
    /// Malformed input or iterations below <see cref="MinimumIterations"/> yield false (optional warning log), never a data-leaking exception.
    /// </summary>
    public static bool TryVerify(string password, string verifier, ILogger? logger = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(verifier) || password is null)
                return false;

            var parts = verifier.Split('$');
            if (parts.Length != 4
                || !string.Equals(parts[0], AlgorithmId, StringComparison.Ordinal))
            {
                logger?.LogWarning("SoftwareProducer PBKDF2 verifier is malformed.");
                return false;
            }

            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations)
                || iterations < MinimumIterations)
            {
                logger?.LogWarning("SoftwareProducer PBKDF2 verifier iterations are invalid or below the minimum.");
                return false;
            }

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                logger?.LogWarning("SoftwareProducer PBKDF2 verifier is malformed.");
                return false;
            }

            if (salt.Length < SaltSizeBytes || expected.Length != HashSizeBytes)
            {
                logger?.LogWarning("SoftwareProducer PBKDF2 verifier is malformed.");
                return false;
            }

            var actual = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                HashSizeBytes);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (Exception)
        {
            logger?.LogWarning("SoftwareProducer PBKDF2 verifier is malformed.");
            return false;
        }
    }
}
