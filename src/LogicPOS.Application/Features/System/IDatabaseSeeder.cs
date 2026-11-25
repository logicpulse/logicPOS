namespace LogicPOS.Application.Features.System;

public interface IDatabaseSeeder
{
    public void ApplyAdditionalSeed();
    public bool ApplyRequiredSeed();

    /// <summary>
    /// Loads the demo sample (articles, places, tables, customers) of the configured module.
    /// Returns false when any part did not load; the caller owns the transaction.
    /// </summary>
    bool ApplyDemoSample();

    /// <summary>
    /// The database already exists when the person confirms the first entry.
    /// Point the sample at the chosen mode and load the country's VAT, payment
    /// methods and document types when that reference data is still missing.
    /// </summary>
    void PrepareConfirmedEntry(string operationMode, string? countryCode2);

    /// <summary>
    /// Ensures mandatory auxiliary lookups (customer types, price types) exist even on older DBs.
    /// </summary>
    void EnsureMandatoryLookups();
    public void ApplyNewSeed();
    public void ApplyRequiredUsersSeed();
    public void EnsureCountrySpecificConfiguration();

    /// <summary>
    /// When a restored database has no portal identity, assigns the login "admin" to one proprietor.
    /// </summary>
    public void EnsurePortalLogin();
}