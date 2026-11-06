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

    /// <summary>Business/Cloud: local API host may run.</summary>
    bool ApiEnabled { get; }

    /// <summary>Business/Cloud: Blazor portal may serve.</summary>
    bool PortalEnabled { get; }

    /// <summary>Business/Cloud: network / print-agent printing.</summary>
    bool NetworkPrintingEnabled { get; }

    /// <summary>Business/Cloud: extra terminals against a shared API.</summary>
    bool MultiTerminalEnabled { get; }

    /// <summary>Machine hardware id shown on the registration form (empty in the public edition).</summary>
    string HardwareId { get; }

    /// <summary>
    /// Reseller name from the licence (set by software key / ERP). Drives POS branding like GTK LicenceReseller.
    /// </summary>
    string Reseller { get; }

    /// <summary>Countries for the registration combo (from ERP AllCountries when available).</summary>
    Task<IReadOnlyList<LicenseCountry>> GetCountriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-check licence with the licensing server (e.g. after ERP soft-delete) before prompting registration.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    Task<LicenseRegistrationResult> RegisterAsync(
        LicenseRegistrationRequest request,
        CancellationToken cancellationToken = default);
}
