using System.Globalization;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Parses a single decimal typed with either '.' or ',' and no thousands separator.
/// Culture is ignored so "2.5" stays 2.5 under pt-PT and "2,5" stays 2.5 under en-US.
/// </summary>
public static class FormDecimal
{
    public static bool TryParse(string? text, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        text = text.Trim();
        var separators = 0;
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (character is '.' or ',')
            {
                separators++;
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
        }

        if (separators > 1)
        {
            return false;
        }

        return decimal.TryParse(
            text.Replace(',', '.'),
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
    }
}
