namespace LogicPOS.Application.Features.System.Users;

/// <summary>Verifies a candidate password against configured SoftwareProducer verifiers.</summary>
public interface ISoftwareProducerPasswordVerifier
{
    /// <summary>Returns true when the password matches the configured verifier (or legacy when enabled).</summary>
    bool VerifyPassword(string? password);
}
