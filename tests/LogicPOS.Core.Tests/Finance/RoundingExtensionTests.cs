using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class RoundingExtensionTests
{
    [Theory]
    [InlineData(0.125, 0.13)]
    [InlineData(-0.125, -0.13)]
    [InlineData(0.145, 0.15)]
    [InlineData(2.675, 2.68)]
    public void Round_half_away_from_zero(decimal input, decimal expected)
        => Assert.Equal(expected, input.Round());

    [Fact]
    public void Round_ignores_division_noise()
        => Assert.Equal(7.50m, 7.4999999999999999999999999m.Round());

    [Fact]
    public void TruncateDecimal_ignores_noise()
        => Assert.Equal(7.50m, 7.4999999999999999999999999m.TruncateDecimal());

    [Fact]
    public void ApplyAgtRounding_examples()
    {
        Assert.Equal(1.23m, 1.2300000000000000000001m.ApplyAgtRounding());
        Assert.Equal(1.48m, 1.477m.ApplyAgtRounding());
    }

    [Fact]
    public void To2DecimalPlacesString_ignores_scale()
    {
        Assert.Equal("1.50", 1.5m.To2DecimalPlacesString());
        Assert.Equal("1.50", 1.500000m.To2DecimalPlacesString());
    }
}
