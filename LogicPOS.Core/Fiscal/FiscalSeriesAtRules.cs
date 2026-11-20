using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Portugal + fiscal module: block issue of fiscal documents until the series has an AT validation code.
/// </summary>
public static class FiscalSeriesAtRules
{
    private static readonly HashSet<string> AtGatedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "FT", "FS", "FR", "NC", "RC",
        "GR", "GT", "GA", "GC", "GD"
    };

    public static bool RequiresAtSeriesCode(string? documentType, FiscalCountry country, bool fiscalModuleAvailable)
    {
        if (country.IsPortugal == false || fiscalModuleAvailable == false)
        {
            return false;
        }

        var type = (documentType ?? string.Empty).Trim();
        return type.Length > 0 && AtGatedDocumentTypes.Contains(type);
    }

    public static bool SeriesHasAtCode(string? atDocCodeValidationSeries)
        => string.IsNullOrWhiteSpace(atDocCodeValidationSeries) == false;

    /// <summary>Refusal text when the gate applies and the series has no AT code; null when issue may continue.</summary>
    public static string? RefusalMessageIfMissing(
        string? documentType,
        string? atDocCodeValidationSeries,
        FiscalCountry country,
        bool fiscalModuleAvailable)
    {
        if (RequiresAtSeriesCode(documentType, country, fiscalModuleAvailable) == false)
        {
            return null;
        }

        return SeriesHasAtCode(atDocCodeValidationSeries)
            ? null
            : "A série ainda não foi comunicada à AT.";
    }
}