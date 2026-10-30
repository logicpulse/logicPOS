using System.Globalization;
using System.Text;

namespace LogicPOS.Core;

/// <summary>Accent/diacritic-insensitive text matching for typed filters.</summary>
public static class TextSearch
{
    public static string Fold(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var form = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(form.Length);
        foreach (var character in form)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static bool Contains(string? haystack, string? needle)
    {
        if (string.IsNullOrEmpty(needle))
        {
            return true;
        }

        if (string.IsNullOrEmpty(haystack))
        {
            return false;
        }

        return Fold(haystack).Contains(Fold(needle), StringComparison.OrdinalIgnoreCase);
    }

    public static bool ContainsAny(string? needle, params string?[] haystacks)
    {
        if (string.IsNullOrEmpty(needle))
        {
            return true;
        }

        foreach (var haystack in haystacks)
        {
            if (Contains(haystack, needle))
            {
                return true;
            }
        }

        return false;
    }
}