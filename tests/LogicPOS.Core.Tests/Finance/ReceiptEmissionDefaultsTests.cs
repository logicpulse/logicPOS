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

    [Fact]
    public void Cash_selects_numerario_even_when_it_is_not_first()
    {
        var card = Method("CC", "Cartão de Crédito");
        var cash = Method(ReceiptEmissionDefaults.CashCode, ReceiptEmissionDefaults.CashName);

        var selected = ReceiptEmissionDefaults.Cash([card, cash]);

        Assert.Same(cash, selected);
    }

    [Fact]
    public void Cash_is_absent_when_only_card_exists()
    {
        var selected = ReceiptEmissionDefaults.Cash([Method("CC", "Cartão de Crédito")]);

        Assert.Null(selected);
        Assert.Contains("Numerário", ReceiptEmissionDefaults.CashUnavailable);
    }

    private static ReceiptPayOption Method(string code, string name)
        => new() { Code = code, Name = name };
}