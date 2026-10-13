using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Core.Tests.Finance;

/// <summary>
/// Parity for country-driven AGT rounding (preview/document path vs standard POS ticket display).
/// </summary>
public class UseAgtRoundingPathTests
{
    [Fact]
    public void Angola_country_enables_agt_line_totals()
    {
        var useAgtRounding = FiscalCountry.Parse("AO").IsAngola;

        Assert.True(useAgtRounding);

        var totals = DocumentLineCalculator.Calculate(10.555m, 1, 0, 14m, useAgtRounding);

        Assert.Equal(10.55m, totals.TotalNet);
        Assert.Equal(1.48m, totals.TotalTax);
        Assert.Equal(12.03m, totals.TotalFinal);
    }

    [Fact]
    public void Portugal_country_uses_standard_line_totals_not_agt()
    {
        var useAgtRounding = FiscalCountry.Parse("PT").IsAngola;

        Assert.False(useAgtRounding);

        var totals = DocumentLineCalculator.Calculate(10.555m, 1, 0, 14m, useAgtRounding);

        Assert.Equal(10.555000m, totals.TotalNet);
        Assert.Equal(1.477700m, totals.TotalTax);
        Assert.Equal(12.032700m, totals.TotalFinal);
    }

    [Fact]
    public void Preview_line_calculation_matches_domain_for_shelf_price_with_vat()
    {
        var useAgtRounding = FiscalCountry.Parse("AO").IsAngola;
        const decimal shelfUnitPrice = 2.50m;
        const decimal quantity = 3m;
        const decimal vatPercentage = 23m;

        var previewTotals = DocumentLineCalculator.CalculateLine(
            shelfUnitPrice,
            priceWithVat: true,
            quantity,
            lineDiscount: 0,
            globalDiscount: 0,
            vatPercentage,
            useAgtRounding);

        var netUnit = DocumentLineCalculator.ToNetUnitPrice(shelfUnitPrice, vatPercentage, priceWithVat: true);
        var domainTotals = DocumentLineCalculator.Calculate(
            netUnit,
            quantity,
            0,
            vatPercentage,
            useAgtRounding);

        Assert.Equal(domainTotals.TotalNet, previewTotals.TotalNet);
        Assert.Equal(domainTotals.TotalTax, previewTotals.TotalTax);
        Assert.Equal(domainTotals.TotalFinal, previewTotals.TotalFinal);
    }

    [Fact]
    public void Document_sale_line_net_price_matches_calculate_line_without_agt()
    {
        const decimal shelfUnitPrice = 9.99m;
        const decimal quantity = 2m;
        const decimal discount = 15m;
        const decimal vatPercentage = 23m;

        var totals = DocumentLineCalculator.CalculateLine(
            shelfUnitPrice,
            priceWithVat: true,
            quantity,
            discount,
            globalDiscount: 0,
            vatPercentage,
            useAgtRounding: false);

        var netUnit = DocumentLineCalculator.ToNetUnitPrice(shelfUnitPrice, vatPercentage, priceWithVat: true);
        var fromNet = DocumentLineCalculator.Calculate(netUnit, quantity, discount, vatPercentage, useAgtRounding: false);

        Assert.Equal(fromNet.TotalNet, totals.TotalNet);
        Assert.Equal(fromNet.TotalTax, totals.TotalTax);
        Assert.Equal(fromNet.TotalFinal, totals.TotalFinal);
    }
}
