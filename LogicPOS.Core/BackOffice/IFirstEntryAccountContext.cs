namespace LogicPOS.Core.BackOffice;

/// <summary>
/// What the first entry knows about the Cloud account, supplied by an adapter (login / ERP).
/// The local edition uses <see cref="LocalFirstEntryAccountContext"/>.
/// Deliberately no account password: the database administrator password is fixed at birth.
/// </summary>
public interface IFirstEntryAccountContext
{
    /// <summary>True for a Cloud account. Independent of <c>DatabaseSettings:UseCloud</c>.</summary>
    bool IsCloudAccount { get; }

    /// <summary>Operation mode on the account, or null when the account has none.</summary>
    string? AccountOperationMode { get; }

    /// <summary>Country of the account (ISO alpha-2, e.g. PT), or null.</summary>
    string? AccountCountryCode2 { get; }

    /// <summary>The "conta em modo demo" mark. Never read by the first entry; present so adapters can show it.</summary>
    bool IsDemoAccount { get; }

    /// <summary>Stores the confirmed mode on the account (also when it equals the previous one).</summary>
    Task SetAccountOperationModeAsync(string mode, CancellationToken cancellationToken = default);
}

/// <summary>Local and public editions: no account.</summary>
public sealed class LocalFirstEntryAccountContext : IFirstEntryAccountContext
{
    public bool IsCloudAccount => false;

    public string? AccountOperationMode => null;

    public string? AccountCountryCode2 => null;

    public bool IsDemoAccount => false;

    public Task SetAccountOperationModeAsync(string mode, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
