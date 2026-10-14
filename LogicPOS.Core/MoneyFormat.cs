using System.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core;

/// <summary>
/// Money display uses the company currency. Parsing does not keep mutable state:
/// one rule for the UI, the session service and the ticket, and ambiguous text is rejected.
/// </summary>
public static class MoneyFormat
{
    public static string Format(decimal value) => value.ToString("C", Culture());

    public static string Format(decimal value, string format) => value.ToString(format, Culture());

    /// <summary>
    /// Portuguese grouping (1.234,56) and a dot decimal (1234.56 or 1.50).
    /// A single separator followed by exactly three digits is rejected when the integer part is not zero ("1.500", "1,500").
    /// "0.125" and "0,125" are 0.125.
    /// </summary>
    public static bool TryParse(string? text, out decimal amount)
    {
        amount = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        text = text.Trim();
        var negative = false;
        if (text[0] is '+' or '-')
        {
            negative = text[0] == '-';
            text = text[1..].Trim();
        }

        if (text.Length == 0 || text.Any(character => char.IsDigit(character) == false && character is not '.' and not ','))
        {
            return false;
        }

        if (text.Any(char.IsDigit) == false)
        {
            return false;
        }

        var comma = text.Contains(',');
        var dot = text.Contains('.');
        string normalized;
        if (comma && dot)
        {
            var decimalSeparator = text.LastIndexOf(',') > text.LastIndexOf('.') ? ',' : '.';
            var groupSeparator = decimalSeparator == ',' ? '.' : ',';
            if (IsGrouped(text, groupSeparator, decimalSeparator) == false)
            {
                return false;
            }

            normalized = text.Replace(groupSeparator.ToString(), string.Empty).Replace(decimalSeparator, '.');
        }
        else if (comma || dot)
        {
            var separator = comma ? ',' : '.';
            var parts = text.Split(separator);
            if (parts.Any(part => part.Length == 0 || part.All(char.IsDigit) == false))
            {
                return false;
            }

            if (parts.Length == 2 && parts[1].Length == 3 && parts[0].All(character => character == '0') == false)
            {
                return false;
            }

            if (parts.Length == 2)
            {
                normalized = parts[0] + "." + parts[1];
            }
            else
            {
                for (var index = 1; index < parts.Length; index++)
                {
                    if (parts[index].Length != 3)
                    {
                        return false;
                    }
                }

                normalized = string.Concat(parts);
            }
        }
        else if (text.All(char.IsDigit) == false)
        {
            return false;
        }
        else
        {
            normalized = text;
        }

        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) == false)
        {
            return false;
        }

        if (negative)
        {
            amount = -amount;
        }

        return true;
    }

    public static string ReadSymbol(LogicPOSDbContext database)
    {
        var parameters = database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && (item.Token == "SYSTEM_CURRENCY_OID" || item.Token == "SYSTEM_CURRENCY"))
            .Select(item => new { item.Token, item.Value })
            .ToList();
        var oid = parameters.FirstOrDefault(item => item.Token == "SYSTEM_CURRENCY_OID")?.Value;
        var acronym = parameters.FirstOrDefault(item => item.Token == "SYSTEM_CURRENCY")?.Value;
        Currency? currency = null;
        if (Guid.TryParse(oid, out var currencyId) && currencyId != Guid.Empty)
        {
            currency = database.Currencies.AsNoTracking()
                .FirstOrDefault(item => item.IsDeleted == false && item.Id == currencyId);
        }

        if (currency is null && string.IsNullOrWhiteSpace(acronym) == false)
        {
            var code = acronym.Trim();
            currency = database.Currencies.AsNoTracking()
                .FirstOrDefault(item => item.IsDeleted == false && item.Acronym == code);
            acronym = code;
        }

        if (string.IsNullOrWhiteSpace(currency?.Symbol) == false)
        {
            return currency.Symbol.Trim();
        }

        return SymbolFor(currency?.Acronym ?? acronym);
    }

    private static readonly CultureInfo Fallback = Build("€");

    private static CultureInfo Culture()
        => AppComposition.Services?.GetService<CompanyCurrency>()?.Culture ?? Fallback;

    internal static CultureInfo ForSymbol(string symbol) => Build(symbol);

    private static bool IsGrouped(string text, char groupSeparator, char decimalSeparator)
    {
        var decimalIndex = text.LastIndexOf(decimalSeparator);
        var fraction = text[(decimalIndex + 1)..];
        var integer = text[..decimalIndex];
        if (fraction.Length == 0 || fraction.All(char.IsDigit) == false || integer.Length == 0)
        {
            return false;
        }

        if (integer.IndexOf(decimalSeparator) >= 0)
        {
            return false;
        }

        var groups = integer.Split(groupSeparator);
        if (groups.Any(group => group.Length == 0 || group.All(char.IsDigit) == false))
        {
            return false;
        }

        for (var index = 1; index < groups.Length; index++)
        {
            if (groups[index].Length != 3)
            {
                return false;
            }
        }

        return true;
    }

    private static CultureInfo Build(string symbol)
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("pt-PT").Clone();
        culture.NumberFormat.CurrencySymbol = symbol;
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

/// <summary>Company currency loaded from SYSTEM_CURRENCY_OID. Not stored on <see cref="MoneyFormat"/>.</summary>
public sealed class CompanyCurrency
{
    private CultureInfo _culture = MoneyFormat.ForSymbol("€");

    public string Symbol { get; private set; } = "€";

    public CultureInfo Culture => _culture;

    public void Reload(LogicPOSDbContext database)
    {
        var symbol = MoneyFormat.ReadSymbol(database);
        if (symbol == Symbol)
        {
            return;
        }

        Symbol = symbol;
        _culture = MoneyFormat.ForSymbol(symbol);
    }
}
