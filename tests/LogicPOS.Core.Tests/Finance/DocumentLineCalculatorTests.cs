using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class DocumentLineCalculatorTests
{
    [Fact]
    public void Price_with_vat_times_quantity_gives_shelf_total()
    {
        var net = 2.50m / 1.23m;
        var t = DocumentLineCalculator.Calculate(net, 3, 0, 23, false);
        Assert.Equal(7.500000m, t.TotalFinal);
    }

    [Fact]
    public void Discounts_10_and_10_combine_to_19()
        => Assert.Equal(19m, DocumentLineCalculator.CombineDiscounts(10, 10));

    [Fact]
    public void Agt_line_truncates_net_and_ceils_tax()
    {
        var t = DocumentLineCalculator.Calculate(10.555m, 1, 0, 14, true);
        Assert.Equal(10.55m, t.TotalNet);
        Assert.Equal(1.48m, t.TotalTax);
        Assert.Equal(12.03m, t.TotalFinal);
    }

    [Fact]
    public void Partial_receipt_tax_splits_sum_to_document_tax()
    {
        decimal final = 100m, tax = 18.70m;
        var a = DocumentLineCalculator.TaxForPayment(final, tax, 0, 33.33m);
        var b = DocumentLineCalculator.TaxForPayment(final, tax, 33.33m, 33.33m);
        var c = DocumentLineCalculator.TaxForPayment(final, tax, 66.66m, 33.34m);
        Assert.Equal(18.70m, a + b + c);
    }

    [Fact]
    public void Settlement_rounds_each_document_before_adding()
    {
        var net = DocumentLineCalculator.SettlementAmount([10.005m, 10.005m], []);
        Assert.Equal(20.02m, net);
    }

    [Fact]
    public void ExtractLineDiscount_undoes_combine()
        => Assert.Equal(10m, DocumentLineCalculator.ExtractLineDiscount(19m, 10m));

    [Fact]
    public void FullCurrencyAmount_multiplies_debt_by_rate()
        => Assert.Equal(1000.00m, DocumentLineCalculator.FullCurrencyAmount(1_000_000m, 0.001m));

    [Fact]
    public void ReceiptAmount_full_currency_pays_whole_debt()
        => Assert.Equal(1000m, DocumentLineCalculator.ReceiptAmount(1000m, 1000m, 1m));
}
