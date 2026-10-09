namespace LogicPOS.Core.Licensing;

/// <summary>
/// Optional license port. The LogicPulse registration implementation stays outside this repository.
/// </summary>
public interface ILicenseModule
{
    bool RegistrationRequired { get; }

    /// <summary>When false, document creation stays inactive (LogicPulse soft lock).</summary>
    bool DocumentsEnabled { get; }

    /// <summary>When false, printing stays inactive (LogicPulse soft lock).</summary>
    bool PrintEnabled { get; }

    /// <summary>Machine hardware id shown on the registration form (empty in the public edition).</summary>
    string HardwareId { get; }

    /// <summary>
    /// Reseller name from the licence (set by software key / ERP). Drives POS branding like GTK LicenceReseller.
    /// </summary>
    string Reseller { get; }

    /// <summary>Countries for the registration combo (from ERP AllCountries when available).</summary>
    Task<IReadOnlyList<LicenseCountry>> GetCountriesAsync(CancellationToken cancellationToken = default);

    Task<LicenseRegistrationResult> RegisterAsync(
        LicenseRegistrationRequest request,
        CancellationToken cancellationToken = default);
}