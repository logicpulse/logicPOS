using System.Globalization;

namespace LogicPOS.Core;

/// <summary>
/// Quantity and weight. One separator is always the decimal separator, with at most four fraction digits.
/// "1,250" and "1.250" are 1.250. Money keeps <see cref="MoneyFormat.TryParse"/>.
/// </summary>
public static class QuantityFormat
{
    public static bool TryParse(string? text, out decimal quantity)
    {
        quantity = 0;
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

        var separators = text.Count(character => character is '.' or ',');
        string normalized;
        if (separators == 0)
        {
            normalized = text;
        }
        else if (separators == 1)
        {
            var split = text.IndexOf('.') >= 0 ? text.IndexOf('.') : text.IndexOf(',');
            var integer = text[..split];
            var fraction = text[(split + 1)..];
            if (integer.Length == 0 || fraction.Length is < 1 or > 4 || integer.All(char.IsDigit) == false || fraction.All(char.IsDigit) == false)
            {
                return false;
            }

            normalized = integer + "." + fraction;
        }
        else
        {
            return false;
        }

        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out quantity) == false)
        {
            return false;
        }

        if (negative)
        {
            quantity = -quantity;
        }

        return true;
    }
}
