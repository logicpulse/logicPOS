using System.Globalization;

namespace LogicPOS.Core.BackOffice;

public enum FormDecimalIssue
{
    None,
    Empty,
    Invalid,
    AmbiguousThousands
}

/// <summary>
/// Decimal text for BackOffice fields. The POS Desktop MoneyFormat must use this same rule later;
/// do not change MoneyFormat in this change.
/// Either '.' or ',' is the decimal separator and thousands groups are not accepted.
/// A separator followed by exactly three digits ("1.500", "1,500") is rejected: it is ambiguous
/// with a thousands group and must not become 1.5 or 1500. "1.5", "1.50", "1.5000" and "1500" are accepted.
/// </summary>
public static class FormDecimal
{
    public const string Rule =
        "Use ponto ou vírgula como decimal, sem separador de milhares. Um separador seguido de exatamente 3 dígitos (1.500 ou 1,500) é ambíguo e não é aceite.";

    public static bool TryParse(string? text, out decimal value)
        => TryParse(text, out value, out _);

    public static bool TryParse(string? text, out decimal value, out FormDecimalIssue issue)
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
        }

        if (separators > 1)
        {
            return false;
        }

        if (separators == 1 && fractionDigits == 3)
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
}
