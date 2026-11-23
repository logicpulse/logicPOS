using System.Globalization;

namespace LogicPOS.Core;

/// <summary>
/// Formats clock/date labels with the app <see cref="UiCulture"/> (cached), not the OS thread culture.
/// </summary>
public static class UiDateTimeFormat
{
    public static CultureInfo ResolveCulture(string? cultureName = null)
    {
        var name = string.IsNullOrWhiteSpace(cultureName) ? UiCulture.Read(null) : cultureName!;
        try
        {
            var culture = CultureInfo.GetCultureInfo(name);
            if (string.Equals(culture.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return culture;
            }
        }
        catch (CultureNotFoundException)
        {
        }

        return CultureInfo.GetCultureInfo("pt-PT");
    }

    /// <summary>Long date from the culture pattern (e.g. en-US: Monday, November 23, 2026).</summary>
    public static string FormatLongDate(DateTime value, string? cultureName = null)
        => value.ToString("D", ResolveCulture(cultureName));

    /// <summary>24-hour clock time — no AM/PM / «da tarde» for Portuguese or English BO clocks.</summary>
    public static string FormatClockTime(DateTime value, string? cultureName = null)
    {
        _ = cultureName; // culture reserved for future ShortTimePattern; clocks stay 24h.
        return value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
