using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities;

namespace LogicPOS.Core.Tests.Finance;

public sealed class ReceiptEmissionDefaultsTests
{
    [Fact]
    public void TakeNextReceiptNumber_uses_series_acronym_and_increments()
    {
        var series = new DocumentSeries { Acronym = "RC2026", NextNumber = 12 };

        var number = ReceiptEmissionDefaults.TakeNextReceiptNumber(series);

        Assert.Equal("RC2026/12", number);
        Assert.Equal(13, series.NextNumber);
    }
}