using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// POS operation mode for the current request.
/// Local mode and requests without a client id use DatabaseSettings.Module.
/// A cloud tenant uses Accounts.PosOperationMode when it is a known token.
/// </summary>
public static class CloudOperationMode
{
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
            return fallback;

        return NormalizeKnown(account.PosOperationMode) ?? fallback;
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
