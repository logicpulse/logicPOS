using System.Globalization;
using LogicPOS.Core;

namespace LogicPOS.Core.Tests;

[Collection(UiCultureCollection.Name)]
public sealed class UiDateTimeFormatTests
{
    [Theory]
    [InlineData("en-US")]
    [InlineData("pt-PT")]
    [InlineData("pt-AO")]
    public void Long_date_uses_culture_pattern_not_fixed_portuguese_de(string cultureName)
    {
        var stamp = new DateTime(2026, 11, 23, 15, 30, 0);
        var text = UiDateTimeFormat.FormatLongDate(stamp, cultureName);
        var culture = CultureInfo.GetCultureInfo(cultureName);
        Assert.Equal(stamp.ToString("D", culture), text);
        if (culture.TwoLetterISOLanguageName == "en")
        {
            Assert.DoesNotContain(" de ", text, StringComparison.Ordinal);
            Assert.Contains("November", text, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Assert.Contains("de", text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("pt-PT")]
    [InlineData("pt-AO")]
    public void Clock_time_is_24_hour_without_am_pm_or_da_tarde(string cultureName)
    {
        var stamp = new DateTime(2026, 11, 23, 15, 30, 45);
        var text = UiDateTimeFormat.FormatClockTime(stamp, cultureName);
        Assert.Equal("15:30:45", text);
        Assert.DoesNotContain("pm", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("am", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tarde", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tt", text, StringComparison.OrdinalIgnoreCase);
    }
}
