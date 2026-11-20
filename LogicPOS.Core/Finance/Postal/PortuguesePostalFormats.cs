using System.Text.RegularExpressions;

namespace LogicPOS.Core.Finance.Postal;

public static partial class PortuguesePostalFormats
{
    private static readonly Regex CompletePattern = CompleteRegex();

    private static readonly Regex PrefixPattern = PrefixRegex();

    public static bool SupportsSuggestion(string? countryCode2)
        => string.Equals(countryCode2, "PT", StringComparison.OrdinalIgnoreCase);

    public static bool TryNormalizeComplete(string? input, out string normalized)
    {
        normalized = (input ?? string.Empty).Trim();
        if (CompletePattern.IsMatch(normalized))
        {
            return true;
        }

        var digits = normalized.Replace("-", string.Empty, StringComparison.Ordinal);
        if (digits.Length == 7 && digits.All(char.IsDigit))
        {
            normalized = digits[..4] + "-" + digits[4..];
            return CompletePattern.IsMatch(normalized);
        }

        normalized = string.Empty;
        return false;
    }

    public static bool IsPrefixOnly(string? input)
        => PrefixPattern.IsMatch((input ?? string.Empty).Trim());

    [GeneratedRegex(@"^\d{4}-\d{3}$", RegexOptions.CultureInvariant)]
    private static partial Regex CompleteRegex();

    [GeneratedRegex(@"^\d{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex PrefixRegex();
}