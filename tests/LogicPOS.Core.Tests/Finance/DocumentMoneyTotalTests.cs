using System.Globalization;
using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class DocumentMoneyTotalTests
{
    [Theory]
    [InlineData(45.505, 45.51)]
    [InlineData(10.005, 10.01)]
    [InlineData(0.125, 0.13)]
    public void Canonical_rounds_half_away_from_zero(decimal stored, decimal expected)
        => Assert.Equal(expected, DocumentMoneyTotal.Canonical(stored));

    [Fact]
    public void Format_uses_canonical_half_away_from_zero()
    {
        Assert.Equal("0.13", DocumentMoneyTotal.Format(0.125m, CultureInfo.InvariantCulture));
        Assert.Equal("45.51", DocumentMoneyTotal.Format(45.505m, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FromLines_sums_then_rounds_once()
    {
        var total = DocumentMoneyTotal.FromLines([10.005m, 10.005m]);
        Assert.Equal(20.02m, total);
    }
}