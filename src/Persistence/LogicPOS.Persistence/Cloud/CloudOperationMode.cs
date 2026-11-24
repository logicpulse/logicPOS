using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// POS operation mode for the current request.
/// Local mode and requests without a client id use DatabaseSettings.Module.
/// A cloud tenant uses Accounts.PosOperationMode when it is a known token.
/// An empty account does not inherit the server module: that file is often restaurant, and the person has not chosen yet.
/// </summary>
public static class CloudOperationMode
{
    /// <summary>Neutral module until the account has a known operation mode.</summary>
    public const string NeutralModule = "default";

    private static readonly HashSet<string> KnownTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "default",
        "backofficemode",
        "restaurant",
        "cafe",
        "retail",
        "parking",
        "bakery",
        "butchery",
        "clothingstore",
        "hardwarestore",
        "seafoodstore",
        "seafoodshop",
        "shoestore"
    };

    public static string? Resolve(
        DatabaseSettings settings,
        ICloudAccountCatalog catalog,
        string? fallbackModule)
    {
        var fallback = NormalizeFallback(fallbackModule);
        if (settings.UseCloud == false)
            return fallback;

        var clientId = TenantContext.AmbientClientId;
        if (string.IsNullOrWhiteSpace(clientId))
            return fallback;

        var account = catalog.Find(clientId);
        if (account.Found == false)
            return NeutralModule;

        return NormalizeKnown(account.PosOperationMode) ?? NeutralModule;
    }

    public static string? NormalizeKnown(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var normalized = token.Trim().ToLowerInvariant();
        return KnownTokens.Contains(normalized) ? normalized : null;
    }

    private static string? NormalizeFallback(string? module) =>
        string.IsNullOrWhiteSpace(module) ? null : module.Trim();
}
