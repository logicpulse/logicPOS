namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// Shared CLI input rules for <c>tools/SoftwareProducerVerifier</c>.
/// </summary>
public static class SoftwareProducerVerifierCliRules
{
    public const string RedirectedInputMessage =
        "SoftwareProducerVerifier requires an interactive terminal. Do not redirect stdin; run without piping the password.";

    /// <summary>
    /// When stdin is redirected, writes a clear message to <paramref name="error"/> and returns a non-zero exit code.
    /// Returns 0 when interactive input is available.
    /// </summary>
    public static int ExitCodeIfInputRedirected(bool isInputRedirected, TextWriter error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (!isInputRedirected)
            return 0;

        error.WriteLine(RedirectedInputMessage);
        return 1;
    }
}
