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
/// BackOffice decimal fields. Parsing delegates to the desktop <see cref="MoneyFormat"/> and <see cref="QuantityFormat"/>
/// so "1.234,56", "1.500.000" and "+1.25" match the POS. Display uses the UI culture (comma in pt-PT).
/// A money value whose natural text is ambiguous ("1,125") is written with four fraction digits ("1,1250")
/// so saving the prefilled field does not fail.
/// </summary>
public static class FormDecimal
{
    public const string DisplayFormat = "0.####";

    public const string QuantityRule =
        "Use ponto ou vírgula como decimal, até 4 casas, sem separador de milhares.";

    public const string MoneyRule =
        "Use ponto ou vírgula como decimal. Um separador seguido de exatamente 3 dígitos com parte inteira diferente de zero (1.500 ou 1,500) é ambíguo e não é aceite. 0.125 é aceite. Milhares usam o formato 1.234,56.";

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

        if (key == "Price")
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

    /// <summary>Tare, weight and minimum stock may be left blank and are stored as zero.</summary>
    public static bool IsOptionalMeasuredKey(string? key)
        => key is "Tare" or "Weight" or "MinimumStock";

    public enum MeasuredEmpty
    {
        NotMeasured,
        StoreZero,
        Required
    }

    public static MeasuredEmpty EmptyPolicy(string? key)
    {
        if (IsOptionalMeasuredKey(key))
        {
            return MeasuredEmpty.StoreZero;
        }

        if (IsQuantityKey(key) || IsMoneyKey(key))
        {
            return MeasuredEmpty.Required;
        }

        return MeasuredEmpty.NotMeasured;
    }

    public static string Format(decimal value, FormDecimalKind kind = FormDecimalKind.Quantity)
    {
        var culture = CultureInfo.CurrentCulture;
        var text = value.ToString(DisplayFormat, culture);
        if (ReadsBack(text, kind, value))
        {
            return text;
        }

        var padded = value.ToString("0.0000", culture);
        if (ReadsBack(padded, kind, value))
        {
            return padded;
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
        var parsed = kind == FormDecimalKind.Money
            ? MoneyFormat.TryParse(text, out value)
            : QuantityFormat.TryParse(text, out value);
        if (parsed)
        {
            issue = FormDecimalIssue.None;
            return true;
        }

        value = 0m;
        issue = Classify(text, kind);
        return false;
    }

    private static bool ReadsBack(string text, FormDecimalKind kind, decimal value)
        => TryParse(text, kind, out var parsed, out _) && parsed == value;

    private static FormDecimalIssue Classify(string? text, FormDecimalKind kind)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return FormDecimalIssue.Empty;
        }

        text = text.Trim();
        if (text[0] is '+' or '-')
        {
            text = text[1..].Trim();
        }

        var separators = text.Count(character => character is '.' or ',');
        if (separators != 1)
        {
            return FormDecimalIssue.Invalid;
        }

        var split = text.IndexOf('.') >= 0 ? text.IndexOf('.') : text.IndexOf(',');
        var integer = text[..split];
        var fraction = text[(split + 1)..];
        if (kind == FormDecimalKind.Quantity && fraction.Length > 4)
        {
            return FormDecimalIssue.TooManyDecimals;
        }

        if (kind == FormDecimalKind.Money
            && fraction.Length == 3
            && integer.Length > 0
            && integer.All(character => character == '0') == false)
        {
            return FormDecimalIssue.AmbiguousThousands;
        }

        return FormDecimalIssue.Invalid;
    }
}
