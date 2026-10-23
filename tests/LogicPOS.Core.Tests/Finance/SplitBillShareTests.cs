using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class SplitBillShareTests
{
    [Fact]
    public void Partial_share_totals_match_the_amount_told_to_the_customer()
    {
        var lines = new[]
        {
            Line(1.99m, 1, vat: 14, withVat: true),
            Line(1.99m, 1, vat: 14, withVat: true),
            Line(1.99m, 1, vat: 14, withVat: true)
        };

        var share = SplitBillShare.ForAmount(lines, 2.98m);

        Assert.Equal(2.98m, share.Sum(item => item.Line.Total));
        Assert.Equal(2.98m, DocumentTotal(share));
    }

    [Fact]
    public void Awkward_quantity_still_bills_the_requested_amount()
    {
        var lines = new[] { Line(3.33m, 7, vat: 23, withVat: true) };
        var share = SplitBillShare.ForAmount(lines, 10.00m);

        Assert.Equal(10.00m, share.Sum(item => item.Line.Total));
        Assert.Equal(10.00m, DocumentTotal(share));
        Assert.True(share[0].Line.Quantity <= 7);
        Assert.Equal(lines[0].NetUnitPrice, share[0].OrderNetUnitPrice);
    }

    [Fact]
    public void Full_bill_keeps_the_original_lines()
    {
        var lines = new[]
        {
            Line(2.50m, 3, vat: 23, withVat: true),
            Line(4m, 1, vat: 23, withVat: false)
        };
        var bill = lines.Sum(line => line.Total);

        var share = SplitBillShare.ForAmount(lines, bill);

        Assert.Equal(bill, share.Sum(item => item.Line.Total));
        Assert.Equal(bill, DocumentTotal(share));
        Assert.Equal(3m, share[0].Line.Quantity);
        Assert.Equal(1m, share[1].Line.Quantity);
    }

    [Fact]
    public void Repeated_payments_keep_the_agreed_share_instead_of_shrinking_the_remainder()
    {
        var line = Line(5000m, 1, vat: 14, withVat: false);
        Assert.Equal(5700m, line.Total);

        var first = SplitBillShare.ForAmount([line], 1900m);
        Assert.Equal(1900m, first.Sum(item => item.Line.Total));
        Assert.InRange(first[0].Line.Quantity, 0.332m, 0.334m);

        var afterFirst = line.WithQuantity(line.Quantity - first[0].Line.Quantity);
        var second = SplitBillShare.ForAmount([afterFirst], 1900m);
        Assert.Equal(1900m, second.Sum(item => item.Line.Total));
        Assert.InRange(second[0].Line.Quantity, 0.300m, 0.360m);

        var afterSecond = afterFirst.WithQuantity(afterFirst.Quantity - second[0].Line.Quantity);
        Assert.InRange(afterSecond.Total, 1800m, 2000m);

        var last = SplitBillShare.ForAmount([afterSecond], 1900m, takeAll: true);
        Assert.Equal(afterSecond.Quantity, last[0].Line.Quantity);
        Assert.Equal(1900m, last.Sum(item => item.Line.Total));
        Assert.Equal(line.NetUnitPrice, last[0].OrderNetUnitPrice);
    }

    [Fact]
    public void Three_equal_shares_leave_exactly_the_unpaid_total()
    {
        var line = Line(58000m, 1, vat: 14, withVat: false);
        Assert.Equal(66120m, line.Total);

        var first = OrderSplitBalance.Remaining([line], 44080m)[0];
        var afterFirst = Apply(line, first);
        Assert.Equal(44080m, afterFirst.Total);

        var removed = line.WithQuantity(line.Quantity - first.Quantity);
        var invoice = SplitBillShare.ForAmount([removed], 22040m, takeAll: true);
        Assert.Equal(22040m, invoice.Sum(item => item.Line.Total));

        var second = OrderSplitBalance.Remaining([afterFirst], 22040m)[0];
        var afterSecond = Apply(afterFirst, second);
        Assert.Equal(22040m, afterSecond.Total);
        Assert.InRange(afterSecond.Quantity, 0.333332m, 0.333334m);
    }

    private static PosTicketLine Apply(PosTicketLine line, OrderLineRemaining remaining)
    {
        var copy = line.WithQuantity(remaining.Quantity);
        copy.NetUnitPrice = remaining.NetUnitPrice;
        return copy;
    }

    [Fact]
    public void Discounted_line_is_billed_at_the_split_amount()
    {
        var lines = new[] { Line(12.50m, 2, vat: 23, withVat: true, discount: 10) };
        var share = SplitBillShare.ForAmount(lines, 8.33m);

        Assert.Equal(8.33m, share.Sum(item => item.Line.Total));
        Assert.Equal(8.33m, DocumentTotal(share));
    }

    private static decimal DocumentTotal(IReadOnlyList<SplitShareLine> share)
        => share.Sum(item => DocumentLineCalculator.CalculateLine(
            item.Line.DisplayUnitPrice,
            item.Line.PriceIncludesVat,
            item.Line.Quantity,
            item.Line.Discount,
            globalDiscount: 0,
            item.Line.VatPercentage,
            useAgtRounding: false).TotalFinal).Round();

    private static PosTicketLine Line(decimal price, decimal quantity, decimal vat, bool withVat, decimal discount = 0)
    {
        var article = new PosArticle(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Artigo",
            "Artigo",
            "A",
            null,
            false,
            price,
            withVat,
            vat,
            quantity,
            discount,
            Guid.NewGuid());
        return new PosTicketLine(article) { Quantity = quantity };
    }
}
