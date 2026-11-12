using LogicPOS.Application.Features.System;
using LogicPOS.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core;

/// <summary>
/// Country-specific fiscal UI. AGT (AGT-FE key / QR logo, AGT series and documents) is Angola only;
/// AT communication (AT credentials, series validation code, «Comunicar à AT») is Portugal only.
/// The fiscal country is appsettings <c>Country</c>, which also picks the <c>db_seeds/{country}</c> of the database.
/// </summary>
public static class FiscalFeatures
{
    public static FiscalCountry Current
        => AppComposition.Services?.GetService<ISystemInformationService>()?.Country ?? FiscalCountry.Portugal;

    public static bool ShowsAgt(FiscalCountry country) => country.IsAngola;

    public static bool ShowsAtCommunication(FiscalCountry country) => country.IsPortugal;

    /// <summary>Preference parameters (company / system settings, company setup) shown for the fiscal country.</summary>
    public static bool IsPreferenceVisible(string? token, FiscalCountry country)
    {
        var key = token?.Trim() ?? string.Empty;
        if (key.StartsWith("AGT_", StringComparison.OrdinalIgnoreCase))
        {
            return ShowsAgt(country);
        }

        if (key.StartsWith("SERVICE_AT_", StringComparison.OrdinalIgnoreCase))
        {
            return ShowsAtCommunication(country);
        }

        return true;
    }

    /// <summary>BackOffice entity fields (series, document types) shown for the fiscal country.</summary>
    public static bool IsEntityFieldVisible(string property, FiscalCountry country) => property switch
    {
        "ATDocCodeValidationSeries" or "WsAtDocument" => ShowsAtCommunication(country),
        _ => true
    };
}
