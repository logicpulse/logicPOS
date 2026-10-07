namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// Active LOGICPOS_CLOUDSERVICE account row used for SQL routing and license overlay.
/// </summary>
public sealed class CloudAccountSnapshot
{
    public static CloudAccountSnapshot NotFound { get; } = new();

    public bool Found { get; init; }

    public string ConnectionString { get; init; } = string.Empty;

    public bool ModuleStocks { get; init; }

    public bool ModuleOnlineInvoicing { get; init; }

    public bool ModuleSms { get; init; }

    /// <summary>Fiscal country from Accounts.idCountry (PT, AO or MZ). Empty when the account has no mapped country.</summary>
    public string? CountryCode2 { get; init; }

    /// <summary>POS operation mode token from Accounts.PosOperationMode. Empty when the account has not chosen one.</summary>
    public string? PosOperationMode { get; init; }
}
