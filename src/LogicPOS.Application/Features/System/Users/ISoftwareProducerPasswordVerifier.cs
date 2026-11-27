namespace LogicPOS.Application.Features.System.Users;

/// <summary>Verifies a candidate password against configured SoftwareProducer verifiers.</summary>
public interface ISoftwareProducerPasswordVerifier
{
    /// <summary>Returns true when the password matches the configured PBKDF2 verifier.</summary>
    bool VerifyPassword(string? password);
}
