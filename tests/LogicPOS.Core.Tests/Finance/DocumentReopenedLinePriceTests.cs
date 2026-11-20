using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class DocumentReopenedLinePriceTests
{
    [Fact]
    public void Price_with_vat_shows_different_unit_and_net_columns()
    {
        const decimal storedNet = 2.032520m;
        const decimal vat = 23m;
        var display = DocumentReopenedLinePrice.FromStoredNet(storedNet, vat, priceIncludesVat: true);
        Assert.True(display.PriceIncludesVat);
        Assert.NotEqual(display.UnitPrice, display.NetUnitPrice);
        Assert.Equal(storedNet, display.NetUnitPrice.ToStoredPrecision());
    }

    [Fact]
    public void Price_without_vat_keeps_unit_and_net_equal()
    {
        const decimal storedNet = 10m;
        var display = DocumentReopenedLinePrice.FromStoredNet(storedNet, 23m, priceIncludesVat: false);
        Assert.False(display.PriceIncludesVat);
        Assert.Equal(storedNet, display.UnitPrice);
        Assert.Equal(storedNet, display.NetUnitPrice);
    }
}