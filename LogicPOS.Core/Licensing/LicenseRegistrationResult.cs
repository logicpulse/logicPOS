namespace LogicPOS.Core.Licensing;

public sealed class LicenseRegistrationResult
{
    private LicenseRegistrationResult(bool succeeded, string? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public bool Succeeded { get; }

    public string? Error { get; }

    public static LicenseRegistrationResult Ok() => new(true, null);

    public static LicenseRegistrationResult Fail(string error) => new(false, error);
}