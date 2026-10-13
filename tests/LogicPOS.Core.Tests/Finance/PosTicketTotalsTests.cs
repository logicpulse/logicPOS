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

    [Fact]
    public void PosTicket_line_total_net_matches_domain_calculate()
    {
        var article = new PosArticle(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Net",
            "Net",
            "N1",
            null,
            false,
            12.345m,
            priceIncludesVat: false,
            vatPercentage: 14m,
            defaultQuantity: 1,
            discount: 5m,
            vatRateId: Guid.NewGuid());

        var line = new PosTicketLine(article) { Quantity = 2 };

        var expected = DocumentLineCalculator
            .Calculate(line.NetUnitPrice, line.Quantity, line.Discount, line.VatPercentage, useAgtRounding: false)
            .TotalNet;

        Assert.Equal(expected, line.TotalNet);
    }

    [Fact]
    public void PosTicket_aggregate_total_matches_domain_sum_of_lines()
    {
        var ticket = new PosTicket();
        ticket.Add(CreateArticle("A", 10m, 23m, priceIncludesVat: false, discount: 0m));
        ticket.Add(CreateArticle("B", 5.50m, 6m, priceIncludesVat: true, discount: 10m));

        var expected = ticket.Lines.Sum(line =>
            DocumentLineCalculator
                .Calculate(line.NetUnitPrice, line.Quantity, line.Discount, line.VatPercentage, useAgtRounding: false)
                .TotalFinal
                .Round());

        Assert.Equal(expected, ticket.Total);
    }

    [Fact]
    public void PosTicket_line_totals_ignore_agt_rounding_until_document_issue()
    {
        var article = CreateArticle("AgtShelf", 10.555m, 14m, priceIncludesVat: false, discount: 0m);
        var line = new PosTicketLine(article) { Quantity = 1 };

        var displayTotals = DocumentLineCalculator.Calculate(
            line.NetUnitPrice,
            line.Quantity,
            line.Discount,
            line.VatPercentage,
            useAgtRounding: false);
        var angolaTotals = DocumentLineCalculator.Calculate(
            line.NetUnitPrice,
            line.Quantity,
            line.Discount,
            line.VatPercentage,
            useAgtRounding: true);

        Assert.Equal(displayTotals.TotalFinal.Round(), line.Total);
        Assert.NotEqual(angolaTotals.TotalNet, displayTotals.TotalNet);
        Assert.NotEqual(angolaTotals.TotalTax, displayTotals.TotalTax);
    }

    private static PosArticle CreateArticle(
        string code,
        decimal price,
        decimal vatPercentage,
        bool priceIncludesVat,
        decimal discount)
        => new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            code,
            code,
            code,
            null,
            false,
            price,
            priceIncludesVat,
            vatPercentage,
            defaultQuantity: 1,
            discount,
            vatRateId: Guid.NewGuid());
}
