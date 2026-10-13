using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class PosTicketTotalsTests
{
    [Fact]
    public void PosTicket_line_total_matches_domain_calculate()
    {
        var article = new PosArticle(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test",
            "Test",
            "T1",
            null,
            false,
            10m,
            priceIncludesVat: false,
            vatPercentage: 23m,
            defaultQuantity: 1,
            discount: 10m,
            vatRateId: Guid.NewGuid());

        var line = new PosTicketLine(article) { Quantity = 3 };

        var expected = DocumentLineCalculator
            .Calculate(line.NetUnitPrice, line.Quantity, line.Discount, line.VatPercentage, useAgtRounding: false)
            .TotalFinal
            .Round();

        Assert.Equal(expected, line.Total);
    }

    [Fact]
    public void PosTicket_shelf_price_with_vat_matches_calculate_line()
    {
        var article = new PosArticle(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Shelf",
            "Shelf",
            "S1",
            null,
            false,
            2.50m,
            priceIncludesVat: true,
            vatPercentage: 23m,
            defaultQuantity: 1,
            discount: 0m,
            vatRateId: Guid.NewGuid());

        var line = new PosTicketLine(article) { Quantity = 3 };
        var expected = DocumentLineCalculator
            .CalculateLine(2.50m, priceWithVat: true, 3, 0, 0, 23m, useAgtRounding: false)
            .TotalFinal
            .Round();

        Assert.Equal(expected, line.Total);
    }
}
