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
    [InlineData("1.250")]
    [InlineData("1,250")]
    [InlineData("2,125")]
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

    [Theory]
    [InlineData("1,250", "1.250")]
    [InlineData("1.250", "1.250")]
    [InlineData("1,2345", "1.2345")]
    [InlineData("12.3456", "12.3456")]
    [InlineData("0,125", "0.125")]
    [InlineData("1250", "1250")]
    public void Parses_quantities_up_to_four_decimals(string text, string expectedText)
    {
        var expected = decimal.Parse(expectedText, CultureInfo.InvariantCulture);
        Assert.True(QuantityFormat.TryParse(text, out var quantity));
        Assert.Equal(expected, quantity);
    }

    [Theory]
    [InlineData("1,23456")]
    [InlineData("1.23456")]
    [InlineData("1.250.000")]
    [InlineData("")]
    public void Rejects_quantities_past_four_decimals(string text)
    {
        Assert.False(QuantityFormat.TryParse(text, out var quantity));
        Assert.Equal(0m, quantity);
    }
}
