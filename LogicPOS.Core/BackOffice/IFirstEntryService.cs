namespace LogicPOS.Core.BackOffice;

/// <summary>Outcome of <see cref="IFirstEntryService.ConfirmAsync"/>.</summary>
public enum FirstEntryConfirmationResult
{
    /// <summary>Mode, company data and confirmation mark were written.</summary>
    Confirmed,

    /// <summary>Required fields missing, an unknown operation mode, or country or currency not found. Nothing was written.</summary>
    Incomplete,

    /// <summary>Invalid Portuguese fiscal number (NIF); nothing was written.</summary>
    InvalidFiscalNumber,

    /// <summary>The first entry is not open (already confirmed, or an existing database). Nothing was written.</summary>
    AlreadyConfirmed,

    /// <summary>The accepted demo sample did not finish loading. Nothing was kept.</summary>
    DemoFailed
}

/// <summary>What the first entry would show: the three steps and their starting values.</summary>
public sealed class FirstEntryPending
{
    public bool IsPending { get; init; }

    /// <summary>Selected operation mode, or null when none is prepared.</summary>
    public string? OperationMode { get; init; }

    public bool DemoAccepted { get; init; }

    public Guid? CountryId { get; init; }

    public Guid? CurrencyId { get; init; }

    public string FiscalNumber { get; init; } = string.Empty;

    public IReadOnlyList<string> Modes { get; init; } = [];

    public IReadOnlyList<CompanySetupChoice> Countries { get; init; } = [];

    public IReadOnlyList<CompanySetupChoice> Currencies { get; init; } = [];
}

/// <summary>One confirmation covering the operation mode, the demo choice and the company data.</summary>
public sealed class FirstEntryRequest
{
    public string? OperationMode { get; init; }

    public bool DemoAccepted { get; init; }

    public Guid? CountryId { get; init; }

    public Guid? CurrencyId { get; init; }

    public string? FiscalNumber { get; init; }
}

/// <summary>What the installer prepared. Preselects the first entry; it is not a confirmation.</summary>
public sealed class FirstEntryDraft
{
    public string? OperationMode { get; init; }

    public bool DemoAccepted { get; init; }

    public Guid? CountryId { get; init; }

    public Guid? CurrencyId { get; init; }

    public string FiscalNumber { get; init; } = string.Empty;
}

/// <summary>
/// The seam for the first entry (primeira entrada). Avalonia, webapp and installer are adapters:
/// they ask <see cref="GetPendingAsync"/> and send one <see cref="ConfirmAsync"/>.
/// </summary>
public interface IFirstEntryService
{
    Task<FirstEntryPending> GetPendingAsync(CancellationToken cancellationToken = default);

    Task<FirstEntryConfirmationResult> ConfirmAsync(
        FirstEntryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the installer draft unless the first entry is no longer open (confirmed, or an existing
    /// database). Returns false and leaves the draft file and the database untouched in that case,
    /// so re-running the installer cannot change the mode or reopen the first entry.
    /// </summary>
    Task<bool> TrySaveDraftAsync(FirstEntryDraft draft, CancellationToken cancellationToken = default);

    /// <summary>
    /// Operation mode the next login should use, or null before the first entry is confirmed.
    /// Cloud account: the account's current mode (it may have changed since the confirmation),
    /// falling back to the confirmed one. Local: the confirmed mode.
    /// </summary>
    Task<string?> GetResolvedOperationModeAsync(CancellationToken cancellationToken = default);

    /// <summary>Currency suggested by the country (its currency code), or null when unknown.</summary>
    Task<Guid?> SuggestCurrencyIdAsync(Guid? countryId, CancellationToken cancellationToken = default);
}
