using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class OrderLineAndSplitTests
{
    [Fact]
    public void Shelf_price_with_vat_matches_document_line_calculator_totals()
    {
        const decimal shelfUnitPrice = 1.30m;
        const decimal quantity = 10m;
        const decimal discount = 0m;
        const decimal vatPercentage = 23m;

        var shelfTotals = DocumentLineCalculator.CalculateLine(
            shelfUnitPrice,
            priceWithVat: true,
            quantity,
            discount,
            globalDiscount: 0,
            vatPercentage,
            useAgtRounding: false);

        var netUnit = DocumentLineCalculator.ToNetUnitPrice(shelfUnitPrice, vatPercentage, priceWithVat: true);
        var orderPathTotals = DocumentLineCalculator.Calculate(
            netUnit,
            quantity,
            discount,
            vatPercentage,
            useAgtRounding: false);

        Assert.Equal(shelfTotals.TotalNet, orderPathTotals.TotalNet);
        Assert.Equal(shelfTotals.TotalTax, orderPathTotals.TotalTax);
        Assert.Equal(shelfTotals.TotalFinal, orderPathTotals.TotalFinal);
    }
}
