using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// Fiscal country for the current request.
/// Local mode and requests without a client id use the API appsettings country.
/// A cloud tenant uses the country stored on the LOGICPOS account (Accounts.idCountry).
/// </summary>
public static class CloudFiscalCountry
{
    public static FiscalCountry Resolve(
        DatabaseSettings settings,
        ICloudAccountCatalog catalog,
        FiscalCountry fallback)
    {
        if (settings.UseCloud == false)
            return fallback;

        var clientId = TenantContext.AmbientClientId;
        if (string.IsNullOrWhiteSpace(clientId))
            return fallback;

        var account = catalog.Find(clientId);
        if (account.Found == false || FiscalCountry.TryParse(account.CountryCode2, out FiscalCountry country) == false)
            return fallback;

        return country;
    }

    public static string? CodeFromCountryName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = name.Trim().ToLowerInvariant().Replace("ç", "c");
        return normalized switch
        {
            "portugal" => "PT",
            "angola" => "AO",
            "mocambique" or "mozambique" => "MZ",
            _ => null
        };
    }
}
