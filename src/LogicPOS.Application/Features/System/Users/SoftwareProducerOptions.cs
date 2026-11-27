namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Configuration for software-producer admin authentication (section <c>SoftwareProducer</c>).
/// </summary>
public sealed class SoftwareProducerOptions
{
    public const string SectionName = "SoftwareProducer";

    /// <summary>
    /// Verifier in PBKDF2 self-describing format
    /// (<c>pbkdf2-sha256$&lt;iterations&gt;$&lt;base64 salt&gt;$&lt;base64 hash&gt;</c>).
    /// Required; when empty or whitespace, producer login is refused.
    /// </summary>
    public string Verifier { get; set; } = string.Empty;
}
