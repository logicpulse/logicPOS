using System.Globalization;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core;

/// <summary>
/// Formats money with the company currency (SYSTEM_CURRENCY), not the UI culture's currency.
/// Number grouping stays Portuguese (pt-PT).
/// </summary>
public static class MoneyFormat
{
    private static CultureInfo _culture = Build("€");

    public static CultureInfo Culture => _culture;

    public static string Symbol => _culture.NumberFormat.CurrencySymbol;

    public static void Apply(string? acronym, string? symbol)
    {
        var resolved = string.IsNullOrWhiteSpace(symbol) ? SymbolFor(acronym) : symbol.Trim();
        _culture = Build(resolved);
    }

    public static void Load(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var acronym = database.PreferenceParameters.AsNoTracking()
                .Where(item => item.Token == "SYSTEM_CURRENCY")
                .Select(item => item.Value)
                .FirstOrDefault();
            string? symbol = null;
            if (string.IsNullOrWhiteSpace(acronym) == false)
            {
                symbol = database.Currencies.AsNoTracking()
                    .Where(item => item.IsDeleted == false && item.Acronym == acronym)
                    .Select(item => item.Symbol)
                    .FirstOrDefault();
            }

            Apply(acronym, symbol);
        }
        catch
        {
            Apply(null, null);
        }
    }

    public static string Format(decimal value) => value.ToString("C", _culture);

    public static string Format(decimal value, string format) => value.ToString(format, _culture);

    /// <summary>
    /// Accepts Portuguese (1.234,56) and invariant (1234.56) amounts typed in the POS.
    /// </summary>
    public static bool TryParse(string? text, out decimal amount)
    {
        amount = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        text = text.Trim();
        const NumberStyles styles = NumberStyles.Number;
        var comma = text.Contains(',');
        var dot = text.Contains('.');
        if (comma)
        {
            return decimal.TryParse(text, styles, CultureInfo.GetCultureInfo("pt-PT"), out amount);
        }

        if (dot)
        {
            return decimal.TryParse(text, styles, CultureInfo.InvariantCulture, out amount);
        }

        return decimal.TryParse(text, styles, CultureInfo.CurrentCulture, out amount)
            || decimal.TryParse(text, styles, CultureInfo.GetCultureInfo("pt-PT"), out amount)
            || decimal.TryParse(text, styles, CultureInfo.InvariantCulture, out amount);
    }

    private static CultureInfo Build(string symbol)
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("pt-PT").Clone();
        culture.NumberFormat.CurrencySymbol = symbol;
        // "n $" — 1.234,56 €
        culture.NumberFormat.CurrencyPositivePattern = 3;
        culture.NumberFormat.CurrencyNegativePattern = 8;
        return culture;
    }

    private static string SymbolFor(string? acronym) => (acronym ?? string.Empty).Trim().ToUpperInvariant() switch
    {
        "EUR" => "€",
        "USD" => "$",
        "GBP" => "£",
        "BRL" => "R$",
        "AOA" => "Kz",
        "MZN" => "MT",
        "CVE" => "$",
        "" => "€",
        var other => other
    };
}
