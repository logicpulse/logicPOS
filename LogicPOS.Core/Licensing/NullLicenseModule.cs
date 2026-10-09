namespace LogicPOS.Core.Licensing;

public sealed class NullLicenseModule : ILicenseModule
{
    public bool RegistrationRequired => false;

    public bool DocumentsEnabled => true;

    public bool PrintEnabled => true;

    public string HardwareId => string.Empty;

    public string Reseller => "LogicPulse";

    public Task<IReadOnlyList<LicenseCountry>> GetCountriesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<LicenseCountry>>([new LicenseCountry(168, "Portugal")]);

    public Task<LicenseRegistrationResult> RegisterAsync(
        LicenseRegistrationRequest request,
        CancellationToken cancellationToken = default)
        => Task.FromResult(LicenseRegistrationResult.Fail("O registo de licença não está disponível nesta edição."));
}