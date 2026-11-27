namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Configuration for software-producer admin authentication (section <c>SoftwareProducer</c>).
/// </summary>
public sealed class SoftwareProducerOptions
{
    public const string SectionName = "SoftwareProducer";

    /// <summary>
    /// Preferred verifier in PBKDF2 self-describing format
    /// (<c>pbkdf2-sha256$&lt;iterations&gt;$&lt;base64 salt&gt;$&lt;base64 hash&gt;</c>).
    /// </summary>
    public string Verifier { get; set; } = string.Empty;

    /// <summary>
    /// Optional legacy PasswordHasher verifier (<c>salt*digest</c> SHA-512). Used only when
    /// <see cref="AcceptLegacy"/> is true. Never hardcode a value; supply via deployment config.
    /// </summary>
    public string? LegacyVerifier { get; set; }

    /// <summary>
    /// When true and <see cref="LegacyVerifier"/> is set, accept the legacy format as a fallback.
    /// Defaults to false.
    /// </summary>
    public bool AcceptLegacy { get; set; }
}
