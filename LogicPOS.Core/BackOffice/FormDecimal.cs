using System.Globalization;

namespace LogicPOS.Core.BackOffice;

public enum FormDecimalKind
{
    Quantity,
    Money
}

public enum FormDecimalIssue
{
    None,
    Empty,
    Invalid,
    AmbiguousThousands,
    TooManyDecimals
}

/// <summary>
/// Decimal text for BackOffice fields. The POS Desktop MoneyFormat must use <see cref="FormDecimalKind.Money"/>
/// later; do not change MoneyFormat in this change.
/// Either '.' or ',' is the decimal separator and thousands groups are not accepted.
/// Quantity, weight and tare accept up to four decimal places and do not use the three-digit rule
/// ("0.125" and "2,125" are 0.125 and 2.125).
/// Money (price and monetary total) rejects a separator followed by exactly three digits only when the
/// integer part is not zero: "1.500" and "1,500" are ambiguous with a thousands group, "0.125" is 0.125.
/// <see cref="Format"/> writes "0.####" (invariant). A money value that would still look ambiguous
/// ("1.125") is written with four decimal places so saving the prefilled text cannot fail.
/// </summary>
public static class FormDecimal
{
    public const string DisplayFormat = "0.####";

    public const string QuantityRule =
        "Use ponto ou vírgula como decimal, até 4 casas, sem separador de milhares.";

    public const string MoneyRule =
        "Use ponto ou vírgula como decimal, sem separador de milhares. Um separador seguido de exatamente 3 dígitos com parte inteira diferente de zero (1.500 ou 1,500) é ambíguo e não é aceite. 0.125 é aceite.";

    public const string Rule = MoneyRule;

    public static bool IsQuantityKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (key is "Quantity" or "DefaultQuantity" or "MinimumStock" or "Tare" or "Weight" or "TotalStock")
        {
            return true;
        }

        return key.StartsWith("ChildQuantity", StringComparison.Ordinal);
    }

    public static bool IsMoneyKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (key is "Price" or "Total" or "TotalFinal" or "Amount")
        {
            return true;
        }

        if (key.Length < 6
            || key.StartsWith("Price", StringComparison.Ordinal) == false
            || key[5] is < '1' or > '5')
        {
            return false;
        }

        var suffix = key.Length == 6 ? string.Empty : key[6..];
        return suffix is "" or "Promotion";
    }

    public static FormDecimalKind KindFor(string? key)
        => IsMoneyKey(key) ? FormDecimalKind.Money : FormDecimalKind.Quantity;

    public static string Format(decimal value, FormDecimalKind kind = FormDecimalKind.Quantity)
    {
        var text = value.ToString(DisplayFormat, CultureInfo.InvariantCulture);
        if (kind == FormDecimalKind.Money && AmbiguousMoney(text))
        {
            text = value.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        return text;
    }

    public static bool TryParse(string? text, out decimal value)
        => TryParse(text, FormDecimalKind.Quantity, out value, out _);

    public static bool TryParse(string? text, out decimal value, out FormDecimalIssue issue)
        => TryParse(text, FormDecimalKind.Quantity, out value, out issue);

    public static bool TryParse(string? text, FormDecimalKind kind, out decimal value)
        => TryParse(text, kind, out value, out _);

    public static bool TryParse(string? text, FormDecimalKind kind, out decimal value, out FormDecimalIssue issue)
    {
        value = 0m;
        issue = FormDecimalIssue.Invalid;
        if (string.IsNullOrWhiteSpace(text))
        {
            issue = FormDecimalIssue.Empty;
            return false;
        }

        text = text.Trim();
        var separators = 0;
        var fractionDigits = 0;
        var nonzeroInteger = false;
        var inFraction = false;
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (character is '.' or ',')
            {
                separators++;
                inFraction = true;
                fractionDigits = 0;
                continue;
            }

            if (character == '-' && index == 0)
            {
                continue;
            }

            if (character is < '0' or > '9')
            {
                return false;
            }

            if (inFraction)
            {
                fractionDigits++;
            }
            else if (character != '0')
            {
                nonzeroInteger = true;
            }
        }

        if (separators > 1)
        {
            return false;
        }

        if (kind == FormDecimalKind.Quantity && separators == 1 && fractionDigits > 4)
        {
            issue = FormDecimalIssue.TooManyDecimals;
            return false;
        }

        if (kind == FormDecimalKind.Money && separators == 1 && fractionDigits == 3 && nonzeroInteger)
        {
            issue = FormDecimalIssue.AmbiguousThousands;
            return false;
        }

        if (decimal.TryParse(
                text.Replace(',', '.'),
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value) == false)
        {
            return false;
        }

        issue = FormDecimalIssue.None;
        return true;
    }

    private static bool AmbiguousMoney(string text)
        => TryParse(text, FormDecimalKind.Money, out _, out var issue) == false
            && issue == FormDecimalIssue.AmbiguousThousands;
}
