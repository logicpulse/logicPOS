using System.Globalization;

namespace LogicPOS.Core.Tests;

public class MoneyFormatTests
{
    [Theory]
    [InlineData("1500", "1500")]
    [InlineData("1,50", "1.50")]
    [InlineData("1.50", "1.50")]
    [InlineData("10,00", "10")]
    [InlineData("0.00", "0")]
    [InlineData("0,05", "0.05")]
    [InlineData("0.125", "0.125")]
    [InlineData("0,125", "0.125")]
    [InlineData("1500.00", "1500")]
    [InlineData("1500,00", "1500")]
    [InlineData("1.234,56", "1234.56")]
    [InlineData("1,234.56", "1234.56")]
    [InlineData("1.500,00", "1500")]
    [InlineData("1,500.00", "1500")]
    [InlineData("1.500.000", "1500000")]
    [InlineData("-1,25", "-1.25")]
    [InlineData("+1.25", "1.25")]
    [InlineData("  10,00 ", "10")]
    public void Parses_unambiguous_amounts(string text, string expectedText)
    {
        var expected = decimal.Parse(expectedText, CultureInfo.InvariantCulture);
        Assert.True(MoneyFormat.TryParse(text, out var amount));
        Assert.Equal(expected, amount);
    }

    [Theory]
    [InlineData("1.500")]
    [InlineData("1,500")]
    [InlineData("12.500")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("1.50.00")]
    [InlineData("1,50,00")]
    [InlineData("1.500 ")]
    public void Rejects_ambiguous_or_invalid_amounts(string text)
    {
        Assert.False(MoneyFormat.TryParse(text, out var amount));
        Assert.Equal(0m, amount);
    }
}
